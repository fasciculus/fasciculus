using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Support;
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
        public LongProgressInfo Progress { get; }
        public EveTrade[] Trades { get; }

        public Task StartAsync();
    }

    public partial class TradeFinder : ObservableObject, ITradeFinder
    {
        [ObservableProperty]
        private LongProgressInfo progress = LongProgressInfo.Start;

        [ObservableProperty]
        private EveTrade[] trades = [];

        private readonly ITradeOptions tradeOptions;
        private readonly IEsiClient esiClient;

        private readonly EveNavigation navigation;

        private readonly IAccumulatingLongProgress longProgress;

        public TradeFinder(ITradeOptions tradeOptions, IEsiClient esiClient, IEveResources resources)
        {
            this.tradeOptions = tradeOptions;
            this.esiClient = esiClient;

            navigation = Tasks.Wait(resources.Navigation);

            longProgress = new AccumulatingLongProgress(OnProgress, 200);
        }

        public Task StartAsync()
        {
            return Tasks.LongRunning(Start);
        }

        private void Start()
        {
            longProgress.Begin(1);

            EveTradeOptions options = new(tradeOptions.Options);
            EveSolarSystem[] solarSystems = GetSolarSystems(options);
            EveMoonStations stations = GetStations(solarSystems);
            EveRegion[] regions = GetRegions(options, solarSystems);
            EveTypes types = GetTypes(options);

            longProgress.Begin(regions.Length * types.Count);
            FindTrades(options, stations, regions, types);
            longProgress.End();
        }

        private void FindTrades(EveTradeOptions options, EveMoonStations stations, EveRegion[] regions, EveTypes types)
        {

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

        private EveTypes GetTypes(EveTradeOptions options)
        {
            EveMarketPrices marketPrices = Tasks.Wait(esiClient.MarketPrices);
            double maxVolume = options.MaxVolumePerType / 10.0;
            double maxPrice = options.MaxIskPerType / 10.0;

            IEnumerable<EveType> types = marketPrices.TradedTypes
                .Where(x => x.Volume > 0 && x.Volume <= maxVolume)
                .Select(x => Tuple.Create(x, marketPrices[x]))
                .Where(x => x.Item2 > 0 && x.Item2 <= maxPrice)
                .Select(x => x.Item1);

            return new(types);
        }

        private void OnProgress(long _)
        {
            Progress = longProgress.Progress;
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
