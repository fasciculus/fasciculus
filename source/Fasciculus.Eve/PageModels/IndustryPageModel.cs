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
        private readonly ISkillProvider skillProvider;

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
        private string salesTaxRate = string.Empty;

        [ObservableProperty]
        private bool ignoreSkills;

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
        private string runsPerDay = string.Empty;

        [ObservableProperty]
        private double jobCost;

        [ObservableProperty]
        private double salesTax;

        [ObservableProperty]
        private double outputVolume;

        [ObservableProperty]
        private EveProductionInput[] inputs = [];

        [ObservableProperty]
        private EveSkillRequirement[] skillRequirements = [];

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

        public IndustryPageModel(IEveSettings settings, IEveProvider provider, IIndustry industry, ISkillProvider skillProvider)
        {
            this.settings = settings.IndustrySettings;
            this.settings.PropertyChanged += OnSettingsChanged;

            this.industry = industry;
            this.industry.PropertyChanged += OnIndustryChanged;

            this.skillProvider = skillProvider;
            this.settings.PropertyChanged += OnSkillsChanged;

            hub = industry.Hub.FullName;

            solarSystems = [.. provider.SolarSystems[EveSecurity.Level.High].Select(x => x.Name).OrderBy(x => x)];
            selectedSolarSystem = this.settings.SolarSystem;

            haulers = [.. EveHaulers.Values.Select(x => x.Caption())];
            selectedHauler = GetHaulerIndex(this.settings.MaxVolume);

            ignoreSkills = this.settings.IgnoreSkills;

            FormatSettings();

            StartCommand.PropertyChanged += OnStartCommandChanged;
        }

        private static int GetHaulerIndex(int volume)
            => EveHaulers.Parse(volume).Index();

        private void OnSettingsChanged(object? sender, PropertyChangedEventArgs ev)
        {
            SelectedSolarSystem = settings.SolarSystem;
            SelectedHauler = GetHaulerIndex(settings.MaxVolume);
            IgnoreSkills = settings.IgnoreSkills;

            FormatSettings();
        }

        private void FormatSettings()
        {
            double sellTaxRate = settings.SalesTaxRate / 10.0;

            SalesTaxRate = sellTaxRate.ToString("0.0") + " %";
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

        private void OnSkillsChanged(object? sender, PropertyChangedEventArgs ev)
        {
            UpdateSkills();
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

                case nameof(IgnoreSkills):
                    settings.IgnoreSkills = IgnoreSkills;
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
                RunsPerDay = "0.00";
                JobCost = 0;
                SalesTax = 0;
                OutputVolume = 0;
                Inputs = [];
            }
            else
            {
                BlueprintPrice = Production.BlueprintPrice;
                JobCost = Production.JobCost;
                SalesTax = Production.SalesTax;
                OutputVolume = Production.OutputVolume;
                Runs = Production.Runs;
                RunsPerDay = Production.RunsPerDay.ToString("0.00");
                Inputs = [.. Production.Inputs];
            }

            UpdateSkills();
        }

        private void UpdateSkills()
        {
            if (Production is null)
            {
                SkillRequirements = [];
            }
            else
            {
                EveSkillRequirements requirements = new(skillProvider, Production.Blueprint.Manufacturing);

                SkillRequirements = requirements.OrderBy(x => x.Name).ToArray();
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
