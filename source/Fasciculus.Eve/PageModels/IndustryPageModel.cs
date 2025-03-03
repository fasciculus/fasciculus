using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Services;
using Fasciculus.Eve.Support;
using Fasciculus.Maui.Collections;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Support.Progressing;
using Fasciculus.Threading;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class IndustryPageModel : MainThreadObservable
    {
        private readonly EveIndustrySettings settings;
        private readonly IIndustry industry;
        private readonly ISkillProvider skillProvider;

        [ObservableProperty]
        public partial string Hub { get; private set; }

        [ObservableProperty]
        public partial List<string> SolarSystems { get; private set; }

        [ObservableProperty]
        public partial string SelectedSolarSystem { get; set; }

        [ObservableProperty]
        public partial List<string> Haulers { get; private set; }

        [ObservableProperty]
        public partial int SelectedHauler { get; set; }

        [ObservableProperty]
        public partial string SalesTaxRate { get; private set; }

        [ObservableProperty]
        public partial bool IgnoreSkills { get; set; }

        [ObservableProperty]
        public partial bool IncludeT2 { get; set; }

        public MainThreadNotifyingEnumerable<EveProduction> Productions { get; }

        [ObservableProperty]
        public partial bool HasProductions { get; private set; }

        [ObservableProperty]
        public partial bool HasProduction { get; private set; }

        [ObservableProperty]
        public partial EveProduction? Production { get; set; }

        [ObservableProperty]
        public partial double BlueprintPrice { get; private set; }

        [ObservableProperty]
        public partial int Runs { get; private set; }

        [ObservableProperty]
        public partial TimeSpan Time { get; private set; }

        [ObservableProperty]
        public partial double JobCost { get; private set; }

        [ObservableProperty]
        public partial double SalesTax { get; private set; }

        [ObservableProperty]
        public partial double OutputVolume { get; private set; }

        [ObservableProperty]
        public partial EveProductionInput[] Inputs { get; private set; }

        [ObservableProperty]
        public partial EveSkillRequirement[] SkillRequirements { get; private set; }

        public ProgressBarProgress BuyOrdersProgress { get; }
        public ProgressBarProgress SellOrdersProgress { get; }
        public ProgressBarProgress MarketPricesProgress { get; }
        public ProgressBarProgress IndustryIndicesProgress { get; }

        [ObservableProperty]
        public partial bool IsRunning { get; private set; }

        [ObservableProperty]
        public partial bool NotRunning { get; private set; }

        public IndustryPageModel(IEveSettings settings, IEveProvider provider, IIndustry industry, ISkillProvider skillProvider,
            EveProgress progress)
        {
            this.settings = settings.IndustrySettings;
            this.settings.PropertyChanged += OnSettingsChanged;

            this.industry = industry;

            this.skillProvider = skillProvider;
            this.settings.PropertyChanged += OnSkillsChanged;

            Hub = industry.Hub.FullName;
            SolarSystems = [.. provider.SolarSystems[EveSecurity.Level.High].Select(x => x.Name).OrderBy(x => x)];
            SelectedSolarSystem = this.settings.SolarSystem;
            Haulers = [.. EveHaulers.Values.Select(x => x.Caption())];
            SelectedHauler = GetHaulerIndex(this.settings.MaxVolume);
            SalesTaxRate = string.Empty;
            IgnoreSkills = this.settings.IgnoreSkills;
            IncludeT2 = this.settings.IncludeT2;

            Inputs = [];
            SkillRequirements = [];

            Productions = new(industry.Productions);
            HasProductions = Productions.Count > 0;
            HasProduction = false;
            Productions.CollectionChanged += OnProductionsChanged;

            BuyOrdersProgress = progress.BuyOrdersProgress;
            SellOrdersProgress = progress.SellOrdersProgress;
            MarketPricesProgress = progress.MarketPricesProgress;
            IndustryIndicesProgress = progress.IndustryIndicesProgress;

            IsRunning = false;
            NotRunning = true;

            FormatSettings();
            UpdateProduction();

            StartCommand.PropertyChanged += OnStartCommandChanged;
        }

        private static int GetHaulerIndex(int volume)
            => EveHaulers.Parse(volume).Index();

        private void OnSettingsChanged(object? sender, PropertyChangedEventArgs ev)
        {
            SelectedSolarSystem = settings.SolarSystem;
            SelectedHauler = GetHaulerIndex(settings.MaxVolume);
            IgnoreSkills = settings.IgnoreSkills;
            IncludeT2 = settings.IncludeT2;

            FormatSettings();
        }

        private void FormatSettings()
        {
            double sellTaxRate = settings.SalesTaxRate / 10.0;

            SalesTaxRate = sellTaxRate.ToString("0.0") + " %";
        }

        private void OnSkillsChanged(object? sender, PropertyChangedEventArgs ev)
        {
            UpdateSkills();
        }

        private void OnProductionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            HasProductions = Productions.Count > 0;

            if (HasProductions)
            {
                Production = Productions[0];
            }
            else
            {
                Production = null;
            }

            UpdateProduction();
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

                case nameof(IncludeT2):
                    settings.IncludeT2 = IncludeT2;
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
                HasProduction = false;

                BlueprintPrice = 0;
                Runs = 0;
                Time = TimeSpan.Zero;
                JobCost = 0;
                SalesTax = 0;
                OutputVolume = 0;
                Inputs = [];
            }
            else
            {
                HasProduction = true;

                BlueprintPrice = Production.BlueprintPrice;
                JobCost = Production.JobCost;
                SalesTax = Production.SalesTax;
                OutputVolume = Production.OutputVolume;
                Runs = Production.Runs;
                Time = TimeSpan.FromSeconds(Production.Time);
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

                SkillRequirements = [.. requirements.OrderBy(x => x.Name)];
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
        private Task CopyToClipboard()
        {
            string[] lines = [.. Inputs.Select(x => $"{x.Type.Name} [{x.Quantity}]")];
            string text = string.Join(Environment.NewLine, lines);

            return Clipboard.SetTextAsync(text).ContinueWith(_ => Task.Delay(200).WaitFor());
        }
    }
}
