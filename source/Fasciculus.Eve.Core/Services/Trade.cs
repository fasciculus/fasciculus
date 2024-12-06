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

            EveTradeOptions options = new(tradeOptions.Options);
            EveSolarSystem[] solarSystems = GetSolarSystems(options);
            EveMoonStations stations = GetStations(solarSystems);
            EveRegion[] regions = GetRegions(options, solarSystems);
            EveMarketPrices marketPrices = Tasks.Wait(esiClient.MarketPrices);
            IEnumerable<Tuple<EveType, int>> types = GetTypes(options, marketPrices);

            workTotal = regions.Length + 1;
            workDone = 0;
            Progress = $"{workDone} / {workTotal}";
            Tasks.Sleep(100);

            Dictionary<int, EveDemandOrSupply> demands = FindDemands(options, marketPrices, types);

            // FindTrades(options, marketPrices, stations, regions, types, demands);

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

        private Dictionary<int, EveDemandOrSupply> FindDemands(EveTradeOptions options, EveMarketPrices marketPrices,
            IEnumerable<Tuple<EveType, int>> types)
        {
            EveRegion region = options.TargetStation.GetRegion();
            long location = options.TargetStation.Id;
            IEnumerable<EveMarketOrder> regionOrders = Tasks.Wait(esiClient.GetMarketOrdersAsync(region, true));
            IEnumerable<EveMarketOrder> targetOrders = regionOrders.Where(x => x.Location == location);

            return [];
        }

        private void FindTrades(EveTradeOptions options, EveMarketPrices marketPrices, EveMoonStations stations, EveRegion[] regions,
            EveTypes types, Dictionary<int, EveDemandOrSupply> demands)
        {
            return;
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

        private static IEnumerable<Tuple<EveType, int>> GetTypes(EveTradeOptions options, EveMarketPrices marketPrices)
        {
            double maxVolume = options.MaxVolumePerType / 10.0;
            double maxPrice = options.MaxIskPerType;

            return marketPrices.TradedTypes
                .Where(x => x.Volume > 0 && x.Volume <= maxVolume)
                .Select(x => Tuple.Create(x, marketPrices[x]))
                .Where(x => x.Item2 > 0 && x.Item2 <= maxPrice)
                .Select(x => x.Item1)
                .Select(x => Tuple.Create(x, maxVolume / x.Volume, maxPrice / marketPrices[x]))
                .Select(x => Tuple.Create(x.Item1, (int)Math.Floor(x.Item2), (int)Math.Floor(x.Item3)))
                .Select(x => Tuple.Create(x.Item1, Math.Min(x.Item2, x.Item3)))
                .Where(x => x.Item2 > 0);
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
