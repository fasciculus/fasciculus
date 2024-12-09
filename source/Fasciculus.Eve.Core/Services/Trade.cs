using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<EveTrade> Trades { get; }

        public Task StartAsync();
    }

    public partial class TradeFinder : ObservableObject, ITradeFinder
    {
        private readonly TaskSafeMutex runningMutex = new();

        [ObservableProperty]
        private string progress = string.Empty;

        private int workTotal;
        private int workDone;
        private readonly TaskSafeMutex workMutex = new();

        public ObservableCollection<EveTrade> Trades { get; } = [];

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
            ClearTrades();

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
            Dictionary<int, EveDemandOrSupply> demandsByType = FindDemands(options, typeQuantities);

            FindTrades(options, stations, regions, typeQuantities, demandsByType);

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

        private void ClearTrades()
        {
            Tasks.Wait(MainThread.InvokeOnMainThreadAsync(() =>
            {
                Trades.Clear();
            }));
        }

        private void AddTrade(EveTrade trade)
        {
            Tasks.Wait(MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (Trades.Count == 10)
                {
                    EveTrade worst = Trades.Last();

                    if (trade.Profit > worst.Profit)
                    {
                        Trades.RemoveAt(9);
                    }
                }

                if (Trades.Count < 10)
                {
                    int index = 0;

                    while (index < Trades.Count && Trades[index].Profit > trade.Profit)
                    {
                        ++index;
                    }

                    Trades.Insert(index, trade);
                }
            }));
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
                .Select(x => FindDemandOrSupply(options.MaxIskPerType * 10.0, station, ordersByItem, x, x => x.OrderByDescending(x => x.Price)))
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

            return stations.SelectMany(x => FindSupplies(options, x, ordersByStation, typeQuantities)).ToArray();
        }

        private static EveDemandOrSupply[] FindSupplies(EveTradeOptions options, EveMoonStation station,
            Dictionary<long, EveMarketOrder[]> ordersByStation, Tuple<EveType, int>[] typeQuantities)
        {
            if (ordersByStation.TryGetValue(station.Id, out EveMarketOrder[]? ordersOfStation))
            {
                Dictionary<int, EveMarketOrder[]> ordersByItem = ordersOfStation
                    .GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToArray());

                return typeQuantities
                    .Select(x => FindDemandOrSupply(options.MaxIskPerType, station, ordersByItem, x, x => x.OrderBy(x => x.Price)))
                    .NotNull().ToArray();
            }

            return [];
        }

        private void FindTrades(EveTradeOptions options, EveMoonStations stations, EveRegion[] regions,
            Tuple<EveType, int>[] typeQuantities, Dictionary<int, EveDemandOrSupply> demandsByType)
            => regions.Apply(x => { FindTrades(options, stations, x, typeQuantities, demandsByType); });

        private void FindTrades(EveTradeOptions options, EveMoonStations stations, EveRegion region, Tuple<EveType, int>[] typeQuantities,
            Dictionary<int, EveDemandOrSupply> demandsByType)
        {
            EveDemandOrSupply[] regionSupplies = FindSupplies(options, typeQuantities, region, stations);

            Dictionary<EveType, EveDemandOrSupply[]> suppliesByType = regionSupplies
                .GroupBy(x => x.Type)
                .ToDictionary(x => x.Key, x => x.ToArray());

            suppliesByType.Apply(x => { FindTrades(x.Key, x.Value, demandsByType); });

            OnWorkDone();
        }

        private void FindTrades(EveType type, EveDemandOrSupply[] typeSupplies, Dictionary<int, EveDemandOrSupply> demandsByType)
        {
            if (demandsByType.TryGetValue(type.Id, out EveDemandOrSupply? demand))
            {
                typeSupplies.Apply(x => { FindTrade(x, demand); });
            }
        }

        private void FindTrade(EveDemandOrSupply supply, EveDemandOrSupply demand)
        {
            int quantity = Math.Min(supply.Quantity, demand.Quantity);
            double profit = quantity * (demand.Price - supply.Price);

            if (profit > 0)
            {
                AddTrade(new(supply, demand, quantity, profit));
            }
        }

        private static EveDemandOrSupply? FindDemandOrSupply(double maxIskPerType, EveMoonStation station,
            Dictionary<int, EveMarketOrder[]> ordersByItem, Tuple<EveType, int> typeQuantity,
            Func<EveMarketOrder[], IOrderedEnumerable<EveMarketOrder>> sort)
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
                    double cost = quantity * price;

                    if (cost > maxIskPerType)
                    {
                        quantity = (int)Math.Floor(maxIskPerType / price);

                        if (quantity > 0)
                        {
                            result = new(station, type, price, quantity);
                        }
                    }
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
