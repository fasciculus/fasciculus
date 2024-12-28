using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Support;
using Fasciculus.Maui.Support.Progressing;
using Fasciculus.Support;
using Fasciculus.Threading;
using Fasciculus.Threading.Synchronization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Services
{
    public interface IIndustry : INotifyPropertyChanged
    {
        public EveStation Hub { get; }

        public ProgressBarProgress BuyProgress { get; }

        //public LongProgressInfo BuyProgressInfo { get; }
        public LongProgressInfo SellProgressInfo { get; }
        public WorkState MarketPricesState { get; }
        public WorkState IndustryIndicesState { get; }

        public EveProduction[] Productions { get; }

        public Task StartAsync();
    }

    public partial class Industry : MainThreadObservable, IIndustry
    {
        private const int SecondsPerDay = 24 * 60 * 60;

        private readonly TaskSafeMutex mutex = new();

        private readonly EveIndustrySettings settings;
        private readonly ISkillProvider skills;
        private readonly IEsiClient esiClient;

        private readonly EveBlueprints blueprints;

        public EveStation Hub { get; }
        private readonly EveRegion hubRegion;

        public ProgressBarProgress BuyProgress { get; }

        //[ObservableProperty]
        //public partial LongProgressInfo BuyProgressInfo { get; set; }

        [ObservableProperty]
        public partial LongProgressInfo SellProgressInfo { get; set; }

        [ObservableProperty]
        public partial WorkState MarketPricesState { get; set; }

        [ObservableProperty]
        public partial WorkState IndustryIndicesState { get; set; }

        //private readonly AccumulatingLongProgress buyProgress;
        private readonly AccumulatingLongProgress sellProgress;

        [ObservableProperty]
        public partial EveProduction[] Productions { get; set; }

        public Industry(IEveSettings settings, ISkillProvider skills, IEsiClient esiClient, IEveProvider provider)
        {
            this.settings = settings.IndustrySettings;
            this.skills = skills;
            this.esiClient = esiClient;

            blueprints = provider.Blueprints;

            Hub = provider.Stations[60003760];
            hubRegion = Hub.GetRegion();

            BuyProgress = new();

            //BuyProgressInfo = LongProgressInfo.Start;
            SellProgressInfo = LongProgressInfo.Start;
            MarketPricesState = WorkState.Pending;
            IndustryIndicesState = WorkState.Pending;

            //buyProgress = new(_ => { BuyProgressInfo = buyProgress!.Progress; });
            sellProgress = new(_ => { SellProgressInfo = sellProgress!.Progress; });

            Productions = [];

            this.settings.PropertyChanged += OnSettingsChanged;
            this.skills.PropertyChanged += OnSkillsChanged;
        }

        public Task StartAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            return Tasks.LongRunning(Start);
        }

        private void Reset()
        {
            Productions = [];
        }

        private void Start()
        {
            Reset();

            BuyProgress.Begin(1);
            sellProgress.Begin(1);
            MarketPricesState = WorkState.Pending;
            IndustryIndicesState = WorkState.Pending;

            EveRegionBuyOrders? regionBuyOrders = Tasks.Wait(esiClient.GetRegionBuyOrdersAsync(hubRegion, BuyProgress));
            EveRegionSellOrders? regionSellOrders = Tasks.Wait(esiClient.GetRegionSellOrdersAsync(hubRegion, sellProgress));

            MarketPricesState = WorkState.Working;
            EveMarketPrices? marketPrices = Tasks.Wait(esiClient.GetMarketPricesAsync());
            MarketPricesState = WorkState.Done;

            IndustryIndicesState = WorkState.Working;
            EveIndustryIndices? industryIndices = Tasks.Wait(esiClient.GetIndustryIndicesAsync());
            IndustryIndicesState = WorkState.Done;

            if (regionBuyOrders is null || regionSellOrders is null || marketPrices is null || industryIndices is null)
            {
                return;
            }

            regionSellOrders = regionSellOrders[EveSecurity.Level.High];
            regionBuyOrders = regionBuyOrders[EveSecurity.Level.High];

            EveStationBuyOrders hubBuyOrders = regionBuyOrders[Hub];
            EveStationSellOrders hubSellOrders = regionSellOrders[Hub];
            EveBlueprint[] candidates = GetCandidates(marketPrices, hubSellOrders);
            double systemCostIndex = industryIndices[Hub.Moon.Planet.SolarSystem];
            double salesTaxRate = settings.SalesTaxRate / 1000.0;
            EveProduction[] productions = CreateProductions(candidates, regionSellOrders, hubSellOrders, hubBuyOrders, marketPrices, systemCostIndex, salesTaxRate);
            int count = Math.Min(20, productions.Length);

            Productions = productions.OrderByDescending(x => x.Profit).Take(count).ToArray();
        }

        private EveProduction[] CreateProductions(EveBlueprint[] blueprints,
            EveRegionSellOrders regionSellOrders,
            EveStationSellOrders sellOrders, EveStationBuyOrders buyOrders,
            EveMarketPrices marketPrices, double systemCostIndex, double salesTaxRate)
        {
            return blueprints
                .Select(x => CreateProduction(x, regionSellOrders, sellOrders, buyOrders, marketPrices, systemCostIndex, salesTaxRate))
                .ToArray();
        }

        private EveProduction CreateProduction(EveBlueprint blueprint, EveRegionSellOrders regionSellOrders,
            EveStationSellOrders sellOrders, EveStationBuyOrders buyOrders,
            EveMarketPrices marketPrices, double systemCostIndex, double salesTaxRate)
        {
            double blueprintPrice = marketPrices[blueprint.Type].AveragePrice;
            EveManufacturing manufacturing = blueprint.Manufacturing;
            int runsByTime = Math.Min(blueprint.MaxRuns, SecondsPerDay / manufacturing.Time);
            double outputVolume = blueprint.Manufacturing.Products.Select(x => x.Quantity * x.Type.Volume).Sum();
            int runsByVolume = (int)Math.Floor(settings.MaxVolume / outputVolume);
            int runs = Math.Min(runsByTime, runsByVolume);
            EveProductionInput[] inputs = CreateInputs(manufacturing.Materials, runs, sellOrders);
            EveProductionOutput[] outputs = CreateOutputs(manufacturing.Products, runs, buyOrders);
            double jobCost = GetJobCost(inputs, marketPrices, systemCostIndex);
            EveProduction production = new(blueprint, blueprintPrice, runs, inputs, outputs, jobCost, salesTaxRate);

            return production;
        }

        private static EveProductionInput[] CreateInputs(IEnumerable<EveMaterial> materials, int runs, EveStationSellOrders sellOrders)
        {
            return materials.Select(x => CreateInput(x, runs, sellOrders)).ToArray();
        }

        private static EveProductionOutput[] CreateOutputs(IEnumerable<EveMaterial> products, int runs, EveStationBuyOrders buyOrders)
        {
            return products.Select(x => CreateOutput(x, runs, buyOrders)).ToArray();
        }

        private static EveProductionInput CreateInput(EveMaterial material, int runs, EveStationSellOrders sellOrders)
        {
            EveType type = material.Type;
            int quantity = runs * material.Quantity;
            double price = sellOrders[type].PriceFor(quantity * 7);
            double cost = quantity * price;

            return new(type, quantity, cost);
        }

        private static EveProductionOutput CreateOutput(EveMaterial product, int runs, EveStationBuyOrders buyOrders)
        {
            EveType type = product.Type;
            int quantity = runs * product.Quantity;
            double price = buyOrders[type].PriceFor(quantity * 7);
            double income = quantity * price;

            return new(type, quantity, income);
        }

        private static double GetJobCost(EveProductionInput[] inputs, EveMarketPrices marketPrices, double systemCostIndex)
        {
            double itemValue = inputs.Select(x => x.Quantity * marketPrices[x.Type].AdjustedPrice).Sum();

            return itemValue * (systemCostIndex + 0.0025 + 0.04);
        }

        private EveBlueprint[] GetCandidates(EveMarketPrices marketPrices, EveStationSellOrders hubSellOrders)
        {
            int maxVolume = settings.MaxVolume;

            EveBlueprint[] candidates = [.. blueprints];

            candidates = [.. candidates.Where(x => x.Manufacturing.Time <= SecondsPerDay)];
            candidates = [.. candidates.Where(x => x.Manufacturing.Products.All(y => y.Type.Volume <= maxVolume))];

            EveBlueprint[] t1 = [.. candidates.Where(x => x.Manufacturing.Products.All(y => y.Type.MetaGroup == 1))];
            EveBlueprint[] t2 = [.. candidates.Where(x => x.Manufacturing.Products.All(y => y.Type.MetaGroup == 2))];

            t1 = [.. t1.Where(x => marketPrices.Contains(x.Type))];
            candidates = settings.IncludeT2 ? [.. t1.Concat(t2)] : t1;

            if (!settings.IgnoreSkills)
            {
                candidates = [.. candidates.Where(x => skills.Fulfills(x.Manufacturing.RequiredSkills))];
            }

            candidates = [.. candidates.Where(x => x.Manufacturing.Materials.All(y => hubSellOrders.Contains(y.Type)))];

            return candidates;
        }
        private void OnSkillsChanged(object? sender, PropertyChangedEventArgs e)
            => Reset();

        private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
            => Reset();
    }
}
