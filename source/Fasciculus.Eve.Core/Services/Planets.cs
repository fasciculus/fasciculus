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
    public interface IPlanetChains
    {
        public EvePlanetChain[] GetChains(EvePlanetSchematicLevel inputLevel, EvePlanetSchematicLevel outputLevel);
    }

    public class PlanetChains : IPlanetChains
    {
        private readonly EvePlanetSchematics schematics;
        private readonly Dictionary<EvePlanetSchematicLevel, Dictionary<EvePlanetSchematicLevel, EvePlanetChain[]>> chains;

        public PlanetChains(IEveProvider provider)
        {
            schematics = provider.PlanetSchematics;
            chains = [];

            AddChains();
        }

        public EvePlanetChain[] GetChains(EvePlanetSchematicLevel inputLevel, EvePlanetSchematicLevel outputLevel)
        {
            if (chains.TryGetValue(outputLevel, out Dictionary<EvePlanetSchematicLevel, EvePlanetChain[]>? byInput))
            {
                if (byInput.TryGetValue(inputLevel, out EvePlanetChain[]? result))
                {
                    return result;
                }
            }

            return [];
        }

        private void AddChains()
        {
            Dictionary<EvePlanetSchematicLevel, EvePlanetChain[]> p13 = [];
            List<EvePlanetChain> list = [];

            foreach (EvePlanetSchematic schematic in schematics.P3)
            {
                foreach (EvePlanetSchematicType input in schematic.Inputs)
                {
                    EvePlanetSchematic inputSchematic = schematics[input.Type];
                    int providedQuantity = inputSchematic.Output.Quantity;
                    int requiredQuantity = input.Quantity;
                    int runs = requiredQuantity / providedQuantity;
                    EvePlanetChain chain = new();

                    list.Add(chain);
                }
            }

            p13.Add(EvePlanetSchematicLevel.P1, [.. list]);
            chains.Add(EvePlanetSchematicLevel.P3, p13);
        }
    }

    public interface IPlanets : INotifyPropertyChanged
    {
        public EveStation Hub { get; }

        public LongProgressInfo BuyProgressInfo { get; }
        public LongProgressInfo SellProgressInfo { get; }

        public EvePlanetProduction[] Productions { get; }

        public Task StartAsync();
    }

    public partial class Planets : MainThreadObservable, IPlanets
    {
        private readonly TaskSafeMutex mutex = new();

        private readonly EvePlanetsSettings settings;
        private readonly IEsiClient esiClient;

        private readonly EvePlanetSchematics schematics;
        private readonly IPlanetChains chains;
        private readonly EvePlanetBaseCosts baseCosts;

        public EveStation Hub { get; }
        private readonly EveRegion hubRegion;

        [ObservableProperty]
        private LongProgressInfo buyProgressInfo = LongProgressInfo.Start;

        [ObservableProperty]
        private LongProgressInfo sellProgressInfo = LongProgressInfo.Start;

        private readonly AccumulatingLongProgress buyProgress;
        private readonly AccumulatingLongProgress sellProgress;

        [ObservableProperty]
        private EvePlanetProduction[] productions = [];

        public Planets(IEveSettings settings, IEsiClient esiClient, IEveProvider provider, IPlanetChains chains)
        {
            this.settings = settings.PlanetsSettings;
            this.settings.PropertyChanged += OnSettingsChanged;

            this.esiClient = esiClient;

            schematics = provider.PlanetSchematics;
            this.chains = chains;
            baseCosts = new(schematics);

            Hub = provider.Stations[60003760];
            hubRegion = Hub.GetRegion();

            buyProgress = new(_ => { BuyProgressInfo = buyProgress!.Progress; });
            sellProgress = new(_ => { SellProgressInfo = sellProgress!.Progress; });
        }

        private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
        {
            Reset();
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

            EveRegionBuyOrders? regionBuyOrders = Tasks.Wait(esiClient.GetRegionBuyOrdersAsync(hubRegion, buyProgress));
            EveRegionSellOrders? regionSellOrders = Tasks.Wait(esiClient.GetRegionSellOrdersAsync(hubRegion, sellProgress));

            if (regionBuyOrders is null || regionSellOrders is null)
            {
                return;
            }

            double customsTaxRate = settings.CustomsTaxRate / 1000.0;
            double salesTaxRate = settings.SalesTaxRate / 1000.0;
            EveStationBuyOrders buyOrders = regionBuyOrders[Hub];
            EveStationSellOrders sellOrders = regionSellOrders[Hub];
            EvePlanetProductions productions = new(schematics, buyOrders, sellOrders, customsTaxRate, salesTaxRate);

            Productions = productions.Take(10).ToArray();
        }

        private EvePlanetInput[] CreateInputs(EvePlanetSchematic output, EvePlanetSchematicLevel importLevel, int runs,
            EveStationSellOrders sellOrders, double customsTaxRate)
        {
            EvePlanetInput[] inputs = output.Inputs
                .Select(x => Tuple.Create(schematics[x.Type], runs * x.Quantity))
                .Select(x => CreateInput(x.Item1, x.Item2, sellOrders, customsTaxRate))
                .ToArray();

            EvePlanetInput[] result = inputs.Where(x => x.Level == importLevel).ToArray();
            EvePlanetInput[] todo = inputs.Where(x => x.Level != importLevel).ToArray();

            while (todo.Length > 0)
            {
                inputs = todo.Select(x => Tuple.Create(schematics[x.Type], x.Quantity))
                    .Select(x => Tuple.Create(x.Item1, runs * x.Item2 / x.Item1.Output.Quantity))
                    .SelectMany(x => CreateInputs(x.Item1, importLevel, x.Item2, sellOrders, customsTaxRate))
                    .ToArray();

                result = result.Concat(inputs.Where(x => x.Level == importLevel)).ToArray();
                todo = inputs.Where(x => x.Level != importLevel).ToArray();
            }

            return result;
        }

        private EvePlanetInput CreateInput(EvePlanetSchematic schematic, int quantity, EveStationSellOrders sellOrders,
            double customsTaxRate)
        {
            EveType type = schematic.OutputType;
            EvePlanetSchematicLevel level = schematic.Level;
            double buyPrice = sellOrders[type].PriceFor(quantity * 7);
            double importTax = quantity * baseCosts[type] * customsTaxRate / 2;

            return new(type, level, quantity, buyPrice, importTax);
        }
    }
}
