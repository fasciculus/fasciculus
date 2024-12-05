using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Support;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

    public partial class TradeFinder : MainThreadObservable, ITradeFinder
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
            longProgress.Begin(0);

            EveMarketPrices marketPrices = Tasks.Wait(esiClient.MarketPrices);
            EveTradeOptions options = new(tradeOptions.Options);
            EveRegion[] regions = GetRegions(options);

            longProgress.Begin(regions.Length);

            foreach (var region in regions)
            {
                Tasks.Wait(Task.Delay(1000));
                longProgress.Report(1);
            }

            longProgress.End();
        }

        private EveRegion[] GetRegions(EveTradeOptions options)
        {
            EveSolarSystem origin = options.TargetStation.Moon.Planet.SolarSystem;

            return navigation
                .InRange(origin, options.MaxDistance, EveSecurity.Level.High)
                .Select(x => x.Constellation.Region)
                .Distinct()
                .ToArray();
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
