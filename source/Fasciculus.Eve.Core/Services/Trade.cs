using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Support;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Services
{
    public interface ITradeOptions
    {
        public EveMoonStation TargetStation { get; }
        public int MaxDistance { get; set; }
        public int MaxVolumePerType { get; set; }
        public int MaxIskPerType { get; set; }
    }

    public partial class TradeOptions : MainThreadObservable, ITradeOptions
    {
        private static readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
        };

        private readonly IEveFileSystem fileSystem;

        public EveMoonStation TargetStation { get; }

        [ObservableProperty]
        private int maxDistance = EveTradeOptions.DefaultMaxDistance;

        [ObservableProperty]
        private int maxVolumePerType = EveTradeOptions.DefaultMaxVolumePerType;

        [ObservableProperty]
        private int maxIskPerType = EveTradeOptions.DefaultMaxIskPerType;

        public TradeOptions(IEveResources resources, IEveFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;

            TargetStation = Tasks.Wait(resources.Universe).NpcStations[EveTradeOptions.DefaultTargetStationId];

            Read();
            Write();
        }

        private void Read()
        {
            EveTradeOptions options = new();

            FileInfo file = fileSystem.TradeOptions;

            if (file.Exists)
            {
                string json = file.ReadAllText();

                options = JsonSerializer.Deserialize<EveTradeOptions>(json) ?? new();
            }

            MaxDistance = options.MaxDistance;
            MaxVolumePerType = options.MaxVolumePerType;
            MaxIskPerType = options.MaxIskPerType;
        }

        private void Write()
        {
            EveTradeOptions options = new()
            {
                TargetStationId = TargetStation.Id,
                MaxDistance = MaxDistance,
                MaxVolumePerType = MaxVolumePerType,
                MaxIskPerType = MaxIskPerType
            };

            string json = JsonSerializer.Serialize(options, serializerOptions);

            fileSystem.TradeOptions.WriteAllText(json);
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

        public IAccumulatingLongProgress longProgress;

        public TradeFinder()
        {
            longProgress = new AccumulatingLongProgress(OnProgress, 200);
        }

        public Task StartAsync()
        {
            return Tasks.LongRunning(Start);
        }

        private void Start()
        {
            longProgress.Begin(1);
            Tasks.Wait(Task.Delay(2000));
            longProgress.End();
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
