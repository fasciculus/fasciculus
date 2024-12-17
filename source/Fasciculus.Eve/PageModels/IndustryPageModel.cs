using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Services;
using Fasciculus.Eve.Support;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Support;
using Fasciculus.Support;
using System.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class IndustryPageModel : MainThreadObservable
    {
        private readonly EveIndustrySettings settings;
        private readonly IIndustry industry;

        [ObservableProperty]
        private string hub;

        [ObservableProperty]
        private List<string> solarSystems;

        [ObservableProperty]
        private string selectedSolarSystem;

        [ObservableProperty]
        private List<string> haulers;

        [ObservableProperty]
        private int selectedHauler;

        [ObservableProperty]
        private bool hasProductions = false;

        [ObservableProperty]
        private EveProduction[] productions = [];

        [ObservableProperty]
        private EveProduction? production;

        [ObservableProperty]
        private double blueprintPrice;

        [ObservableProperty]
        private int runs;

        [ObservableProperty]
        private double jobCost;

        [ObservableProperty]
        private double outputVolume;

        [ObservableProperty]
        private EveProductionInput[] inputs = [];

        [ObservableProperty]
        private LongProgressInfo buyProgress = LongProgressInfo.Start;

        [ObservableProperty]
        private LongProgressInfo sellProgress = LongProgressInfo.Start;

        [ObservableProperty]
        private WorkState marketPricesState = WorkState.Pending;

        [ObservableProperty]
        private WorkState industryIndicesState = WorkState.Pending;

        [ObservableProperty]
        private bool isRunning = false;

        [ObservableProperty]
        private bool notRunning = true;

        public IndustryPageModel(IEveSettings settings, IEveProvider provider, IIndustry industry)
        {
            this.settings = settings.IndustrySettings;
            this.settings.PropertyChanged += OnSettingsChanged;

            this.industry = industry;
            this.industry.PropertyChanged += OnIndustryChanged;

            hub = industry.Hub.FullName;

            solarSystems = [.. provider.SolarSystems.Select(x => x.Name).OrderBy(x => x)];
            selectedSolarSystem = this.settings.SolarSystem;

            haulers = [.. EveHaulers.Values.Select(x => x.Caption())];
            selectedHauler = GetHaulerIndex(this.settings.MaxVolume);

            StartCommand.PropertyChanged += OnStartCommandChanged;
        }

        private static int GetHaulerIndex(int volume)
            => EveHaulers.Parse(volume).Index();

        private void OnSettingsChanged(object? sender, PropertyChangedEventArgs ev)
        {
            SelectedSolarSystem = settings.SolarSystem;
            SelectedHauler = GetHaulerIndex(settings.MaxVolume);
        }

        private void OnIndustryChanged(object? sender, PropertyChangedEventArgs ev)
        {
            BuyProgress = industry.BuyProgressInfo;
            SellProgress = industry.SellProgressInfo;

            MarketPricesState = industry.MarketPricesState;
            IndustryIndicesState = industry.IndustryIndicesState;

            Productions = industry.Productions;
            HasProductions = Productions.Length > 0;
            Production = HasProductions ? Productions[0] : null;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs ev)
        {
            base.OnPropertyChanged(ev);

            switch (ev.PropertyName)
            {
                case nameof(SelectedSolarSystem):
                    settings.SolarSystem = SelectedSolarSystem;
                    break;

                case nameof(SelectedHauler):
                    settings.MaxVolume = EveHaulers.Values[SelectedHauler].Volume();
                    break;

                case nameof(Production):
                    UpdateProduction();
                    break;
            }
        }

        private void UpdateProduction()
        {
            if (Production is null)
            {
                BlueprintPrice = 0;
                Runs = 0;
                JobCost = 0;
                OutputVolume = 0;
                Inputs = [];
            }
            else
            {
                BlueprintPrice = Production.BlueprintPrice;
                JobCost = Production.JobCost;
                OutputVolume = Production.OutputVolume;
                Runs = Production.Runs;
                Inputs = [.. Production.Inputs];
            }
        }

        private void OnStartCommandChanged(object? sender, PropertyChangedEventArgs e)
        {
            IsRunning = StartCommand.IsRunning;
            NotRunning = !StartCommand.IsRunning;
        }

        [RelayCommand]
        private Task Start()
        {
            return industry.StartAsync();
        }
    }
}
