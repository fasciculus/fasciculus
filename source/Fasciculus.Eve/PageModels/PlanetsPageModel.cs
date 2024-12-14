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
        private string sellTaxRate = string.Empty;

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
        private bool isRunning;

        public PlanetsPageModel(IEveSettings settings, IPlanets planets)
        {
            this.settings = settings.PlanetsSettings;
            this.settings.PropertyChanged += OnSettingsChanged;

            this.planets = planets;
            this.planets.PropertyChanged += OnPlanetsChanged;

            hub = planets.Hub.FullName;

            FormatSettings();
        }

        private void FormatSettings()
        {
            double customsTaxRate = settings.CustomsTaxRate / 10.0;
            double sellTaxRate = settings.SellTaxRate / 10.0;

            CustomsTaxRate = customsTaxRate.ToString("0.0") + " %";
            SellTaxRate = sellTaxRate.ToString("0.0") + " %";
        }

        private void OnSettingsChanged(object? sender, PropertyChangedEventArgs ev)
        {
            FormatSettings();
        }

        private void OnPlanetsChanged(object? sender, PropertyChangedEventArgs ev)
        {
            SellProgress = planets.SellProgressInfo;
            BuyProgress = planets.BuyProgressInfo;
            Productions = planets.Productions;
            HasProductions = Productions.Length > 0;

            if (HasProductions)
            {
                Production = Productions[0];
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs ev)
        {
            base.OnPropertyChanged(ev);

            if (ev.PropertyName == nameof(Production))
            {
                Inputs = Production is null ? [] : [.. Production.Inputs];
            }
        }

        private void SetRunning(bool running)
        {
            IsRunning = running;
        }

        [RelayCommand]
        private Task Start()
        {
            Production = null;
            SetRunning(true);

            return planets.StartAsync()
                .ContinueWith(_ => { SetRunning(false); });
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
            settings.SellTaxRate = Math.Max(0, settings.SellTaxRate - 1);
        }

        [RelayCommand]
        private void IncrementSellTaxRate()
        {
            settings.SellTaxRate += 1;
        }
    }
}
