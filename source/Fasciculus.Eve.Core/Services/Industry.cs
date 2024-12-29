using Fasciculus.Collections;
using Fasciculus.Eve.Models;
using Fasciculus.Support;
using Fasciculus.Threading;
using Fasciculus.Threading.Synchronization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Services
{
    public interface IIndustry
    {
        public EveStation Hub { get; }

        public ObservableSortedSet<EveProduction> Productions { get; }

        public Task StartAsync();
    }

    public partial class Industry : IIndustry
    {
        public class ProductionComparer : IComparer<EveProduction>
        {
            public int Compare(EveProduction? x, EveProduction? y)
            {
                EveProduction lhs = Cond.NotNull(x);
                EveProduction rhs = Cond.NotNull(y);
                int result = rhs.Profit.CompareTo(lhs.Profit);

                if (result == 0)
                {
                    result = rhs.Margin.CompareTo(lhs.Margin);
                }

                if (result == 0)
                {
                    result = lhs.BlueprintPrice.CompareTo(rhs.BlueprintPrice);
                }

                if (result == 0)
                {
                    result = lhs.Name.CompareTo(rhs.Name);
                }

                return result;
            }
        }

        private const int SecondsPerDay = 24 * 60 * 60;
        private static readonly ProductionComparer Comparer = new();

        private readonly TaskSafeMutex mutex = new();

        private readonly EveIndustrySettings settings;
        private readonly ISkillProvider skills;
        private readonly IEsiClient esiClient;
        private readonly IEveProvider provider;
        private readonly EveProgress progress;

        private readonly EveBlueprints blueprints;

        private EveRegionBuyOrders regionBuyOrders = EveRegionBuyOrders.Empty;
        private EveRegionSellOrders regionSellOrders = EveRegionSellOrders.Empty;
        private EveMarketPrices marketPrices = EveMarketPrices.Empty;
        private EveIndustryIndices industryIndices = EveIndustryIndices.Empty;

        public EveStation Hub { get; }

        private EveStationBuyOrders hubBuyOrders;
        private EveStationSellOrders hubSellOrders;

        private double systemCostIndex;
        private double salesTaxRate;

        private EveBlueprint[] candidates = [];

        public ObservableSortedSet<EveProduction> Productions { get; }

        public Industry(IEveSettings settings, ISkillProvider skills, IEsiClient esiClient, IEveProvider provider, EveProgress progress)
        {
            this.settings = settings.IndustrySettings;
            this.skills = skills;
            this.esiClient = esiClient;
            this.provider = provider;
            this.progress = progress;

            blueprints = provider.Blueprints;

            Hub = provider.Stations[60003760];
            hubBuyOrders = new([], Hub);
            hubSellOrders = new([], Hub);

            Productions = new(Comparer);

            this.settings.PropertyChanged += (_, _) => { Reset(); };
            this.skills.PropertyChanged += (_, _) => { Reset(); };
        }

        public Task StartAsync()
            => Tasks.LongRunning(Start);

        private void Reset()
            => Productions.Clear();

        private void Start()
        {
            using Locker locker = Locker.Lock(mutex);

            Reset();

            RefreshEsiData();
            RefreshParameters();
            RefreshCandidates();

            FindProductions();
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

            hubBuyOrders = regionBuyOrders[Hub];
            hubSellOrders = regionSellOrders[Hub];
        }

        private void RefreshParameters()
        {
            EveSolarSystems solarSystems = provider.SolarSystems;
            string solarSystemName = solarSystems.Contains(settings.SolarSystem) ? settings.SolarSystem : "Jita";
            EveSolarSystem solarSystem = solarSystems[solarSystemName];

            systemCostIndex = industryIndices[solarSystem];
            salesTaxRate = settings.SalesTaxRate / 1000.0;
        }

        private void RefreshCandidates()
        {
            int maxVolume = settings.MaxVolume;

            candidates = [.. blueprints];

            candidates = [.. candidates.Where(x => x.Manufacturing.Time <= SecondsPerDay)];
            candidates = [.. candidates.Where(x => x.Manufacturing.Products.All(y => y.Type.Volume <= maxVolume))];

            EveBlueprint[] buyable = [.. candidates.Where(x => marketPrices.Contains(x.Type))];
            EveBlueprint[] t2 = [.. candidates.Where(x => x.Manufacturing.Products.All(y => y.Type.MetaGroup == 2))];

            candidates = settings.IncludeT2 ? [.. buyable.Concat(t2)] : buyable;

            if (!settings.IgnoreSkills)
            {
                candidates = [.. candidates.Where(x => skills.Fulfills(x.Manufacturing.RequiredSkills))];
            }

            candidates = [.. candidates.Where(x => x.Manufacturing.Materials.All(y => hubSellOrders.Contains(y.Type)))];
        }

        private void AddProduction(EveProduction production)
        {
            if (Productions.Count < 20)
            {
                Productions.Add(production);
            }
            else
            {
                if (Comparer.Compare(production, Productions.Last()) < 0)
                {
                    Productions.Remove(Productions.Last());
                    Productions.Add(production);
                }
            }
        }

        private void FindProductions()
        {
            foreach (EveBlueprint blueprint in candidates)
            {
                EveProduction production = CreateProduction(blueprint);

                if (production.Profit > 0)
                {
                    AddProduction(production);
                }
            }
        }

        private EveProduction CreateProduction(EveBlueprint blueprint)
        {
            double blueprintPrice = marketPrices[blueprint.Type].AveragePrice;
            EveManufacturing manufacturing = blueprint.Manufacturing;
            int runsByTime = Math.Min(blueprint.MaxRuns, SecondsPerDay / manufacturing.Time);
            double outputVolume = blueprint.Manufacturing.Products.Select(x => x.Quantity * x.Type.Volume).Sum();
            int runsByVolume = (int)Math.Floor(settings.MaxVolume / outputVolume);
            int runs = Math.Min(runsByTime, runsByVolume);
            EveProductionInput[] inputs = [.. manufacturing.Materials.Select(x => CreateInput(x, runs))];
            EveProductionOutput[] outputs = [.. manufacturing.Products.Select(x => CreateOutput(x, runs))];
            double jobCost = GetJobCost(inputs);

            return new(blueprint, blueprintPrice, runs, inputs, outputs, jobCost, salesTaxRate);
        }

        private EveProductionInput CreateInput(EveMaterial material, int runs)
        {
            EveType type = material.Type;
            int quantity = runs * material.Quantity;
            double price = hubSellOrders[type].PriceFor(quantity * 7);
            double cost = quantity * price;

            return new(type, quantity, cost);
        }

        private EveProductionOutput CreateOutput(EveMaterial product, int runs)
        {
            EveType type = product.Type;
            int quantity = runs * product.Quantity;
            double price = hubBuyOrders[type].PriceFor(quantity * 7);
            double income = quantity * price;

            return new(type, quantity, income);
        }

        private double GetJobCost(EveProductionInput[] inputs)
        {
            double itemValue = inputs.Select(x => x.Quantity * marketPrices[x.Type].AdjustedPrice).Sum();

            return itemValue * (systemCostIndex + 0.0025 + 0.04);
        }
    }
}
