using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Services
{
    public interface ITradeOptions : INotifyPropertyChanged
    {
        public EveTradeOptions Options { get; set; }

        public EveTradeOptions Create(int maxDistance, int maxVolumePerType, int maxIskPerType);
    }

    public partial class TradeOptions : MainThreadObservable, ITradeOptions
    {
        private readonly IEveFileSystem fileSystem;

        private readonly EveMoonStations stations;

        [ObservableProperty]
        private EveTradeOptions options;

        public TradeOptions(IEveResources resources, IEveFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;

            stations = Tasks.Wait(resources.Universe).NpcStations;
            options = new(stations);

            Read();
        }

        public EveTradeOptions Create(int maxDistance, int maxVolumePerType, int maxIskPerType)
        {
            EveTradeOptions.Data data = new(maxDistance, maxVolumePerType, maxIskPerType);

            return new(data, stations);
        }

        private void Read()
        {
            FileInfo file = fileSystem.TradeOptions;
            EveTradeOptions? options = null;

            if (file.Exists)
            {
                using Stream stream = file.OpenRead();

                options = new(stream, stations);
            }

            if (options is not null)
            {
                Options = options;
            }
        }

        private void Write()
        {
            FileInfo file = fileSystem.TradeOptions;
            using Stream stream = file.OpenWrite();

            Options.Write(stream);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            Write();
        }
    }

    public interface ITradeFinder : INotifyPropertyChanged
    {
        public string Progress { get; }
        public EveTrade[] Trades { get; }

        public Task StartAsync();
    }

    public partial class TradeFinder : ObservableObject, ITradeFinder
    {
        private TaskSafeMutex runningMutex = new();

        [ObservableProperty]
        private string progress = string.Empty;

        private int workTotal;
        private int workDone;
        private readonly TaskSafeMutex workMutex = new();

        [ObservableProperty]
        private EveTrade[] trades = [];
        private readonly TaskSafeMutex tradesMutex = new();

        private readonly ITradeOptions tradeOptions;
        private readonly IEsiClient esiClient;

        private readonly EveNavigation navigation;

        public TradeFinder(ITradeOptions tradeOptions, IEsiClient esiClient, IEveResources resources)
        {
            this.tradeOptions = tradeOptions;
            this.esiClient = esiClient;

            navigation = Tasks.Wait(resources.Navigation);
        }

        public Task StartAsync()
        {
            using Locker locker = Locker.Lock(runningMutex);

            return Tasks.LongRunning(Start);
        }

        private void Start()
        {
            Trades = [];
            Tasks.Sleep(100);

            EveMarketPrices marketPrices = Tasks.Wait(esiClient.MarketPrices);
            EveTradeOptions options = new(tradeOptions.Options);
            EveSolarSystem[] solarSystems = GetSolarSystems(options);
            EveRegion[] regions = GetRegions(options, solarSystems);

            workTotal = regions.Length + 1;
            workDone = 0;
            Progress = $"{workDone} / {workTotal}";
            Tasks.Sleep(100);

            EveMoonStations stations = GetStations(solarSystems);
            Tuple<EveType, int>[] typeQuantities = GetTypeQuantities(options, marketPrices);
            Dictionary<int, EveDemandOrSupply> demands = FindDemands(options, typeQuantities);

            FindTrades(options, stations, regions, typeQuantities);

            workDone = workTotal;
            Progress = $"{workDone} / {workTotal}";
            Tasks.Sleep(100);
        }

        private void OnWorkDone()
        {
            using Locker locker = Locker.Lock(workMutex);

            ++workDone;
            Progress = $"{workDone} / {workTotal}";
            //Tasks.Sleep(5);
        }

        private void AddTrade(EveTrade trade)
        {
            using Locker locker = Locker.Lock(tradesMutex);

            bool changed = false;

            if (Trades.Length < 10)
            {
                IEnumerable<EveTrade> trades = Trades.Append(trade).OrderByDescending(x => x.Profit);
                int count = Math.Min(10, trades.Count());

                Trades = trades.Take(count).ToArray();
                changed = true;
            }
            else
            {
                EveTrade worst = Trades.Last();

                if (trade.Profit > worst.Profit)
                {
                    IEnumerable<EveTrade> trades = Trades.Append(trade).OrderByDescending(x => x.Profit);

                    Trades = trades.Take(10).ToArray();
                    changed = true;
                }
            }

            if (changed)
            {
                Tasks.Sleep(500);
            }

        }

        private Dictionary<int, EveDemandOrSupply> FindDemands(EveTradeOptions options, Tuple<EveType, int>[] typeQuantities)
        {
            EveMoonStation station = options.TargetStation;
            long location = station.Id;
            EveRegion region = options.TargetStation.GetRegion();
            EveMarketOrders regionOrders = Tasks.Wait(esiClient.GetMarketOrdersAsync(region, true));
            EveMarketOrder[] stationOrders = regionOrders.Where(x => x.Location == location).ToArray();
            Dictionary<int, EveMarketOrder[]> ordersByItem = stationOrders.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToArray());

            Dictionary<int, EveDemandOrSupply> result = typeQuantities
                .Select(x => FindDemandOrSupply(station, ordersByItem, x, x => x.OrderByDescending(x => x.Price)))
                .NotNull()
                .ToDictionary(x => x.Type.Id);

            OnWorkDone();

            return result;
        }

        private EveDemandOrSupply[] FindSupplies(EveTradeOptions options, Tuple<EveType, int>[] typeQuantities,
            EveRegion region, EveMoonStations stations)
        {
            EveMarketOrders regionOrders = Tasks.Wait(esiClient.GetMarketOrdersAsync(region, false));

            Dictionary<long, EveMarketOrder[]> ordersByStation = regionOrders
                .GroupBy(x => x.Location)
                .ToDictionary(x => x.Key, x => x.ToArray());

            return [];
        }

        private void FindTrades(EveTradeOptions options, EveMoonStations stations, EveRegion[] regions,
            Tuple<EveType, int>[] typeQuantities)
        {
            foreach (EveRegion region in regions)
            {
                EveDemandOrSupply[] supplies = FindSupplies(options, typeQuantities, region, stations);

                OnWorkDone();
            }

            return;
        }

        private static EveDemandOrSupply? FindDemandOrSupply(EveMoonStation station, Dictionary<int, EveMarketOrder[]> ordersByItem,
            Tuple<EveType, int> typeQuantity, Func<EveMarketOrder[], IOrderedEnumerable<EveMarketOrder>> sort)
        {
            EveDemandOrSupply? result = null;
            EveType type = typeQuantity.Item1;
            int typeId = type.Id;

            if (ordersByItem.TryGetValue(typeId, out EveMarketOrder[]? unsortedOrders))
            {
                IEnumerable<EveMarketOrder> sortedOrders = sort(unsortedOrders);
                int desiredQuantity = typeQuantity.Item2;
                int quantity = 0;
                double price = 0;

                for (IEnumerator<EveMarketOrder> e = sortedOrders.GetEnumerator(); quantity < desiredQuantity && e.MoveNext();)
                {
                    EveMarketOrder order = e.Current;

                    quantity = Math.Min(desiredQuantity, quantity + order.Quantity);
                    price += order.Price;
                }

                if (quantity > 0)
                {
                    result = new(station, type, price, quantity);
                }
            }

            return result;
        }

        private EveSolarSystem[] GetSolarSystems(EveTradeOptions options)
        {
            EveSolarSystem origin = options.TargetStation.Moon.Planet.SolarSystem;

            return navigation.InRange(origin, options.MaxDistance, EveSecurity.Level.High).ToArray();
        }

        private static EveMoonStations GetStations(IEnumerable<EveSolarSystem> solarSystems)
        {
            IEnumerable<EveMoonStation> stations = solarSystems
                .SelectMany(x => x.Planets)
                .SelectMany(x => x.Moons)
                .SelectMany(x => x.Stations);

            return new(stations);
        }

        private static EveRegion[] GetRegions(EveTradeOptions options, IEnumerable<EveSolarSystem> solarSystems)
        {
            EveRegion targetRegion = options.TargetStation.GetRegion();

            return solarSystems.Select(x => x.Constellation.Region).Append(targetRegion).Distinct().ToArray();
        }

        private static Tuple<EveType, int>[] GetTypeQuantities(EveTradeOptions options, EveMarketPrices marketPrices)
        {
            double maxVolume = options.MaxVolumePerType;
            double maxPrice = options.MaxIskPerType;

            return marketPrices.TradedTypes
                .Where(x => x.Volume > 0 && x.Volume <= (maxVolume / 10.0))
                .Select(x => Tuple.Create(x, marketPrices[x]))
                .Where(x => x.Item2 > 0 && x.Item2 <= maxPrice)
                .Select(x => x.Item1)
                .Select(x => Tuple.Create(x, maxVolume / x.Volume, maxPrice / marketPrices[x]))
                .Select(x => Tuple.Create(x.Item1, (int)Math.Floor(x.Item2), (int)Math.Floor(x.Item3)))
                .Select(x => Tuple.Create(x.Item1, Math.Min(x.Item2, x.Item3)))
                .Where(x => x.Item2 > 0)
                .ToArray();
        }
    }

    public static class TradeServices
    {
        public static IServiceCollection AddTrade(this IServiceCollection services)
        {
            services.AddEveFileSystem();
            services.AddEveResources();

            services.TryAddSingleton<ITradeOptions, TradeOptions>();
            services.TryAddSingleton<ITradeFinder, TradeFinder>();

            return services;
        }
    }
}
