using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Support;
using Fasciculus.Support;
using Fasciculus.Threading;
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

        public LongProgressInfo BuyProgressInfo { get; }
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

        [ObservableProperty]
        private LongProgressInfo buyProgressInfo = LongProgressInfo.Start;

        [ObservableProperty]
        private LongProgressInfo sellProgressInfo = LongProgressInfo.Start;

        [ObservableProperty]
        private WorkState marketPricesState = WorkState.Pending;

        [ObservableProperty]
        private WorkState industryIndicesState = WorkState.Pending;

        private readonly AccumulatingLongProgress buyProgress;
        private readonly AccumulatingLongProgress sellProgress;

        [ObservableProperty]
        private EveProduction[] productions = [];

        public Industry(IEveSettings settings, ISkillProvider skills, IEsiClient esiClient, IEveProvider provider)
        {
            this.settings = settings.IndustrySettings;
            this.skills = skills;
            this.esiClient = esiClient;

            blueprints = provider.Blueprints;

            Hub = provider.Stations[60003760];
            hubRegion = Hub.GetRegion();

            buyProgress = new(_ => { BuyProgressInfo = buyProgress!.Progress; });
            sellProgress = new(_ => { SellProgressInfo = sellProgress!.Progress; });

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

            buyProgress.Begin(1);
            sellProgress.Begin(1);
            MarketPricesState = WorkState.Pending;
            IndustryIndicesState = WorkState.Pending;

            EveRegionBuyOrders? regionBuyOrders = Tasks.Wait(esiClient.GetRegionBuyOrdersAsync(hubRegion, buyProgress));
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

            EveBlueprint[] candidates = GetCandidates(regionSellOrders, marketPrices, settings.IgnoreSkills);
            EveStationBuyOrders buyOrders = regionBuyOrders[Hub];
            EveStationSellOrders sellOrders = regionSellOrders[Hub];
            double systemCostIndex = industryIndices[Hub.Moon.Planet.SolarSystem];
            EveProduction[] productions = CreateProductions(candidates, regionSellOrders, sellOrders, buyOrders, marketPrices, systemCostIndex);
            int count = Math.Min(20, productions.Length);

            Productions = productions.OrderByDescending(x => x.Profit).Take(count).ToArray();
        }

        private static EveProduction[] CreateProductions(EveBlueprint[] blueprints,
            EveRegionSellOrders regionSellOrders,
            EveStationSellOrders sellOrders, EveStationBuyOrders buyOrders,
            EveMarketPrices marketPrices, double systemCostIndex)
        {
            return blueprints
                .Select(x => CreateProduction(x, regionSellOrders, sellOrders, buyOrders, marketPrices, systemCostIndex))
                //.Where(x => x.Cost < 1_000_000_000)
                .Where(x => x.Income < 1_000_000_000)
                .ToArray();
        }

        private static EveProduction CreateProduction(EveBlueprint blueprint, EveRegionSellOrders regionSellOrders,
            EveStationSellOrders sellOrders, EveStationBuyOrders buyOrders,
            EveMarketPrices marketPrices, double systemCostIndex)
        {
            double blueprintPrice = GetBlueprintPrice(blueprint, regionSellOrders, marketPrices);
            EveManufacturing manufacturing = blueprint.Manufacturing;
            int runs = Math.Min(blueprint.MaxRuns, SecondsPerDay / manufacturing.Time);
            EveProductionInput[] inputs = CreateInputs(manufacturing.Materials, runs, sellOrders);
            EveProductionOutput[] outputs = CreateOutputs(manufacturing.Products, runs, buyOrders);
            double jobCost = GetJobCost(inputs, marketPrices, systemCostIndex);

            return new(blueprint, blueprintPrice, runs, inputs, outputs, jobCost);
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

        private static double GetBlueprintPrice(EveBlueprint blueprint, EveRegionSellOrders regionSellOrders, EveMarketPrices marketPrices)
        {
            EveType type = blueprint.Type;
            double regionPrice = regionSellOrders[type].PriceFor(1);
            double averagePrice = marketPrices.Contains(type) ? marketPrices[type].AveragePrice : double.MaxValue;

            return Math.Min(regionPrice, averagePrice);
        }

        private static double GetJobCost(EveProductionInput[] inputs, EveMarketPrices marketPrices, double systemCostIndex)
        {
            double itemValue = inputs.Select(x => x.Quantity * marketPrices[x.Type].AdjustedPrice).Sum();

            return itemValue * (systemCostIndex + 0.0025 + 0.04);
        }

        private EveBlueprint[] GetCandidates(EveRegionSellOrders regionSellOrders, EveMarketPrices marketPrices, bool ignoreSkills)
        {
            int maxVolume = settings.MaxVolume;

            return blueprints
                .Where(x => x.Manufacturing.Time <= SecondsPerDay)
                .Where(x => regionSellOrders[x.Type].Count > 0 || marketPrices.Contains(x.Type))
                .Where(x => x.Manufacturing.Products.All(y => y.Type.Volume <= maxVolume))
                .Where(x => x.Manufacturing.Materials.All(y => marketPrices.Contains(y.Type)))
                .Where(x => ignoreSkills || skills.Fulfills(x.Manufacturing.RequiredSkills))
                .ToArray();
        }
        private void OnSkillsChanged(object? sender, PropertyChangedEventArgs e)
            => Reset();

        private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
            => Reset();
    }
}
