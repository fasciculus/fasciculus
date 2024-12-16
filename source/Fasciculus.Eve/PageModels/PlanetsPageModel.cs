using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Support;
using System.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class PlanetsPageModel : MainThreadObservable
    {
        private readonly EvePlanetsSettings settings;
        private readonly IPlanets planets;

        [ObservableProperty]
        private string hub;

        [ObservableProperty]
        private string customsTaxRate = string.Empty;

        [ObservableProperty]
        private string salesTaxRate = string.Empty;

        [ObservableProperty]
        private LongProgressInfo buyProgress = LongProgressInfo.Start;

        [ObservableProperty]
        private LongProgressInfo sellProgress = LongProgressInfo.Start;

        [ObservableProperty]
        private bool hasProductions = false;

        [ObservableProperty]
        private EvePlanetProduction[] productions = [];

        [ObservableProperty]
        private EvePlanetProduction? production = null;

        [ObservableProperty]
        private EvePlanetInput[] inputs = [];

        [ObservableProperty]
        private bool isRunning = false;

        [ObservableProperty]
        private bool notRunning = true;

        public PlanetsPageModel(IEveSettings settings, IPlanets planets)
        {
            this.settings = settings.PlanetsSettings;
            this.settings.PropertyChanged += OnSettingsChanged;

            this.planets = planets;
            this.planets.PropertyChanged += OnPlanetsChanged;

            hub = planets.Hub.FullName;

            FormatSettings();

            StartCommand.PropertyChanged += OnStartCommandChanged;
        }

        private void FormatSettings()
        {
            double customsTaxRate = settings.CustomsTaxRate / 10.0;
            double sellTaxRate = settings.SalesTaxRate / 10.0;

            CustomsTaxRate = customsTaxRate.ToString("0.0") + " %";
            SalesTaxRate = sellTaxRate.ToString("0.0") + " %";
        }

        private void OnSettingsChanged(object? sender, PropertyChangedEventArgs ev)
        {
            FormatSettings();
        }

        private void OnStartCommandChanged(object? sender, PropertyChangedEventArgs e)
        {
            IsRunning = StartCommand.IsRunning;
            NotRunning = !StartCommand.IsRunning;
        }

        private void OnPlanetsChanged(object? sender, PropertyChangedEventArgs ev)
        {
            BuyProgress = planets.BuyProgressInfo;
            SellProgress = planets.SellProgressInfo;

            Productions = planets.Productions;
            HasProductions = Productions.Length > 0;
            Production = HasProductions ? Productions[0] : null;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs ev)
        {
            base.OnPropertyChanged(ev);

            if (ev.PropertyName == nameof(Production))
            {
                Inputs = Production is null ? [] : [.. Production.Inputs];
            }
        }

        [RelayCommand]
        private Task Start()
        {
            return planets.StartAsync();
        }

        [RelayCommand]
        private void DecrementCustomsTaxRate()
        {
            settings.CustomsTaxRate = Math.Max(0, settings.CustomsTaxRate - 10);
        }

        [RelayCommand]
        private void IncrementCustomsTaxRate()
        {
            settings.CustomsTaxRate += 10;
        }

        [RelayCommand]
        private void DecrementSellTaxRate()
        {
            settings.SalesTaxRate = Math.Max(0, settings.SalesTaxRate - 1);
        }

        [RelayCommand]
        private void IncrementSellTaxRate()
        {
            settings.SalesTaxRate += 1;
        }
    }
}
