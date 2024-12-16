using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Maui.ComponentModel;
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

        private readonly AccumulatingLongProgress buyProgress;
        private readonly AccumulatingLongProgress sellProgress;

        [ObservableProperty]
        private EveProduction[] productions = [];

        public Industry(IEveSettings settings, ISkillProvider skills, IEsiClient esiClient, IDataProvider data, IUniverseProvider universe)
        {
            this.settings = settings.IndustrySettings;
            this.skills = skills;
            this.esiClient = esiClient;

            blueprints = data.Blueprints;

            Hub = universe.Stations[60003760];
            hubRegion = Hub.GetRegion();

            buyProgress = new(_ => { BuyProgressInfo = buyProgress!.Progress; });
            sellProgress = new(_ => { SellProgressInfo = sellProgress!.Progress; });
        }

        public Task StartAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            return Tasks.LongRunning(Start);
        }

        private void Start()
        {
            EveRegionBuyOrders? regionBuyOrders = Tasks.Wait(esiClient.GetRegionBuyOrdersAsync(hubRegion, buyProgress));
            EveRegionSellOrders? regionSellOrders = Tasks.Wait(esiClient.GetRegionSellOrdersAsync(hubRegion, sellProgress));

            if (regionBuyOrders is null || regionSellOrders is null)
            {
                return;
            }

            EveBlueprint[] candidates = GetCandidates(regionSellOrders);
            EveStationBuyOrders buyOrders = regionBuyOrders[Hub];
            EveStationSellOrders sellOrders = regionSellOrders[Hub];
            EveProduction[] productions = CreateProductions(candidates, sellOrders, buyOrders);
            int count = Math.Min(20, productions.Length);

            Productions = productions.OrderByDescending(x => x.Profit).Take(count).ToArray();
        }

        private static EveProduction[] CreateProductions(IEnumerable<EveBlueprint> blueprints,
            EveStationSellOrders sellOrders, EveStationBuyOrders buyOrders)
        {
            return blueprints
                .Select(x => CreateProduction(x, sellOrders, buyOrders))
                .Where(x => x.Cost < 1_000_000_000)
                .Where(x => x.Income < 1_000_000_000)
                .ToArray();
        }

        private static EveProduction CreateProduction(EveBlueprint blueprint, EveStationSellOrders sellOrders, EveStationBuyOrders buyOrders)
        {
            EveManufacturing manufacturing = blueprint.Manufacturing;
            int runs = Math.Min(blueprint.MaxRuns, SecondsPerDay / manufacturing.Time);
            EveProductionInput[] inputs = CreateInputs(manufacturing.Materials, runs, sellOrders);
            EveProductionOutput[] outputs = CreateOutputs(manufacturing.Products, runs, buyOrders);

            return new(blueprint, inputs, outputs);
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

        private EveBlueprint[] GetCandidates(EveRegionSellOrders regionSellOrders)
        {
            int maxVolume = settings.MaxVolume;

            return blueprints
                .Where(x => x.Manufacturing.Time <= SecondsPerDay)
                .Where(x => regionSellOrders[x.Type].Count > 0)
                .Where(x => x.Manufacturing.Products.All(y => y.Type.Volume <= maxVolume))
                .Where(x => skills.Fulfills(x.Manufacturing.RequiredSkills))
                .ToArray();
        }
    }
}
