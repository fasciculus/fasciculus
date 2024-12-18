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
        public partial string Hub { get; private set; }

        [ObservableProperty]
        public partial string CustomsTaxRate { get; private set; }

        [ObservableProperty]
        public partial string SalesTaxRate { get; private set; }

        [ObservableProperty]
        public partial int ProductionLines { get; private set; }

        [ObservableProperty]
        public partial int HoursPerDay { get; private set; }

        [ObservableProperty]
        public partial LongProgressInfo BuyProgress { get; private set; }

        [ObservableProperty]
        public partial LongProgressInfo SellProgress { get; private set; }

        [ObservableProperty]
        public partial bool HasProductions { get; private set; }

        [ObservableProperty]
        public partial EvePlanetProduction[] Productions { get; private set; }

        [ObservableProperty]
        public partial EvePlanetProduction? Production { get; set; }

        [ObservableProperty]
        public partial EvePlanetInput[] Inputs { get; private set; }

        [ObservableProperty]
        public partial bool IsRunning { get; set; }

        [ObservableProperty]
        public partial bool NotRunning { get; set; }

        public PlanetsPageModel(IEveSettings settings, IPlanets planets)
        {
            this.settings = settings.PlanetsSettings;
            this.settings.PropertyChanged += OnSettingsChanged;

            this.planets = planets;
            this.planets.PropertyChanged += OnPlanetsChanged;
            Hub = planets.Hub.FullName;
            CustomsTaxRate = string.Empty;
            SalesTaxRate = string.Empty;
            ProductionLines = this.settings.ProductionLines;
            HoursPerDay = this.settings.HoursPerDay;
            BuyProgress = LongProgressInfo.Start;
            SellProgress = LongProgressInfo.Start;
            HasProductions = false;
            Productions = [];
            Production = null;
            Inputs = [];
            IsRunning = false;
            NotRunning = true;

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
            ProductionLines = settings.ProductionLines;
            HoursPerDay = settings.HoursPerDay;

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
        private void DecrementSalesTaxRate()
        {
            settings.SalesTaxRate = Math.Max(0, settings.SalesTaxRate - 1);
        }

        [RelayCommand]
        private void IncrementSalesTaxRate()
        {
            settings.SalesTaxRate += 1;
        }

        [RelayCommand]
        private void DecrementProductionLines()
        {
            settings.ProductionLines = Math.Max(1, settings.ProductionLines - 1);
        }

        [RelayCommand]
        private void IncrementProductionLines()
        {
            settings.ProductionLines = Math.Min(4, settings.ProductionLines + 1);
        }

        [RelayCommand]
        private void DecrementHoursPerDay()
        {
            settings.HoursPerDay = Math.Max(1, settings.HoursPerDay - 1);
        }

        [RelayCommand]
        private void IncrementHoursPerDay()
        {
            settings.HoursPerDay = Math.Min(23, settings.HoursPerDay + 1);
        }
    }
}
