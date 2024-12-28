using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Maui.ComponentModel;
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
        private readonly EveProgress progress;

        private readonly EveBlueprints blueprints;

        private EveRegionBuyOrders regionBuyOrders = EveRegionBuyOrders.Empty;
        private EveRegionSellOrders regionSellOrders = EveRegionSellOrders.Empty;
        private EveMarketPrices marketPrices = EveMarketPrices.Empty;
        private EveIndustryIndices industryIndices = EveIndustryIndices.Empty;

        public EveStation Hub { get; }

        [ObservableProperty]
        public partial EveProduction[] Productions { get; set; }

        public Industry(IEveSettings settings, ISkillProvider skills, IEsiClient esiClient, IEveProvider provider, EveProgress progress)
        {
            this.settings = settings.IndustrySettings;
            this.skills = skills;
            this.esiClient = esiClient;
            this.progress = progress;

            blueprints = provider.Blueprints;

            Hub = provider.Stations[60003760];

            Productions = [];

            this.settings.PropertyChanged += OnSettingsChanged;
            this.skills.PropertyChanged += OnSkillsChanged;
        }

        public Task StartAsync()
        {
            return Tasks.LongRunning(Start);
        }

        private void Reset()
        {
            Productions = [];
        }

        private void Start()
        {
            using Locker locker = Locker.Lock(mutex);

            Reset();

            RefreshEsiData();

            EveStationBuyOrders hubBuyOrders = regionBuyOrders[Hub];
            EveStationSellOrders hubSellOrders = regionSellOrders[Hub];
            EveBlueprint[] candidates = GetCandidates(hubSellOrders);
            double costIndex = industryIndices[Hub.Moon.Planet.SolarSystem];
            double salesTaxRate = settings.SalesTaxRate / 1000.0;

            EveProduction[] productions = CreateProductions(candidates, hubSellOrders, hubBuyOrders, costIndex, salesTaxRate);
            int count = Math.Min(20, productions.Length);

            Tasks.Sleep(333);

            Productions = productions.OrderByDescending(x => x.Profit).Take(count).ToArray();
        }

        private void RefreshEsiData()
        {
            progress.BuyOrdersProgress.Begin(1);
            progress.SellOrdersProgress.Begin(1);
            progress.MarketPricesProgress.Begin(2);
            progress.IndustryIndicesProgress.Begin(2);

            EveRegion hubRegion = Hub.GetRegion();

            regionBuyOrders = Tasks.Wait(esiClient.GetRegionBuyOrdersAsync(hubRegion, progress.BuyOrdersProgress)) ?? EveRegionBuyOrders.Empty;
            regionSellOrders = Tasks.Wait(esiClient.GetRegionSellOrdersAsync(hubRegion, progress.SellOrdersProgress)) ?? EveRegionSellOrders.Empty;

            progress.MarketPricesProgress.Report(1);
            marketPrices = Tasks.Wait(esiClient.GetMarketPricesAsync()) ?? EveMarketPrices.Empty;
            progress.MarketPricesProgress.End();

            progress.IndustryIndicesProgress.Report(1);
            industryIndices = Tasks.Wait(esiClient.GetIndustryIndicesAsync()) ?? EveIndustryIndices.Empty;
            progress.IndustryIndicesProgress.End();

            regionBuyOrders = regionBuyOrders[EveSecurity.Level.High];
            regionSellOrders = regionSellOrders[EveSecurity.Level.High];
        }

        private EveProduction[] CreateProductions(EveBlueprint[] blueprints, EveStationSellOrders sellOrders, EveStationBuyOrders buyOrders,
            double systemCostIndex, double salesTaxRate)
        {
            return blueprints
                .Select(x => CreateProduction(x, sellOrders, buyOrders, systemCostIndex, salesTaxRate))
                .ToArray();
        }

        private EveProduction CreateProduction(EveBlueprint blueprint,
            EveStationSellOrders sellOrders, EveStationBuyOrders buyOrders,
            double systemCostIndex, double salesTaxRate)
        {
            double blueprintPrice = marketPrices[blueprint.Type].AveragePrice;
            EveManufacturing manufacturing = blueprint.Manufacturing;
            int runsByTime = Math.Min(blueprint.MaxRuns, SecondsPerDay / manufacturing.Time);
            double outputVolume = blueprint.Manufacturing.Products.Select(x => x.Quantity * x.Type.Volume).Sum();
            int runsByVolume = (int)Math.Floor(settings.MaxVolume / outputVolume);
            int runs = Math.Min(runsByTime, runsByVolume);
            EveProductionInput[] inputs = CreateInputs(manufacturing.Materials, runs, sellOrders);
            EveProductionOutput[] outputs = CreateOutputs(manufacturing.Products, runs, buyOrders);
            double jobCost = GetJobCost(inputs, systemCostIndex);
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

        private double GetJobCost(EveProductionInput[] inputs, double systemCostIndex)
        {
            double itemValue = inputs.Select(x => x.Quantity * marketPrices[x.Type].AdjustedPrice).Sum();

            return itemValue * (systemCostIndex + 0.0025 + 0.04);
        }

        private EveBlueprint[] GetCandidates(EveStationSellOrders hubSellOrders)
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
