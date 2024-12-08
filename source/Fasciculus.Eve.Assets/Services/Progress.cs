using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Support;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel;

namespace Fasciculus.Eve.Assets.Services
{
    public enum PendingToDone
    {
        Pending,
        Working,
        Done
    }

    public enum DownloadSdeStatus
    {
        Pending,
        Downloading,
        Downloaded,
        NotModified
    }

    public interface IAssetsProgress : INotifyPropertyChanged
    {
        public DownloadSdeStatus DownloadSdeInfo { get; }
        public IProgress<DownloadSdeStatus> DownloadSdeProgress { get; }

        public LongProgressInfo ExtractSdeInfo { get; }
        public IAccumulatingLongProgress ExtractSdeProgress { get; }

        public PendingToDone ParseNamesInfo { get; }
        public IProgress<PendingToDone> ParseNamesProgress { get; }

        public PendingToDone ParseMarketGroupsInfo { get; }
        public IProgress<PendingToDone> ParseMarketGroupsProgress { get; }

        public PendingToDone ParseTypesInfo { get; }
        public IProgress<PendingToDone> ParseTypesProgress { get; }

        public PendingToDone ParseStationOperationsInfo { get; }
        public IProgress<PendingToDone> ParseStationOperationsProgress { get; }

        public PendingToDone ParseNpcCorporationsInfo { get; }
        public IProgress<PendingToDone> ParseNpcCorporationsProgress { get; }

        public LongProgressInfo ParseRegionsInfo { get; }
        public IAccumulatingLongProgress ParseRegionsProgress { get; }

        public LongProgressInfo ParseConstellationsInfo { get; }
        public IAccumulatingLongProgress ParseConstellationsProgress { get; }

        public LongProgressInfo ParseSolarSystemsInfo { get; }
        public IAccumulatingLongProgress ParseSolarSystemsProgress { get; }

        public IProgress<PendingToDone> ConvertDataProgress { get; }

        public IProgress<PendingToDone> ConvertUniverseProgress { get; }

        public IProgress<PendingToDone> CreateConnectionsProgress { get; }

        public IAccumulatingLongProgress CreateDistancesProgress { get; }
        public IAccumulatingLongProgress CopyImagesProgress { get; }

        public IProgress<PendingToDone> CreateImagesProgress { get; }

        public IAccumulatingProgress<List<FileInfo>> CreateResourcesProgress { get; }
    }

    public partial class AssetsProgress : MainThreadObservable, IAssetsProgress
    {
        private readonly IProgressCollector progressCollector;

        [ObservableProperty]
        private DownloadSdeStatus downloadSdeInfo = DownloadSdeStatus.Pending;
        public IProgress<DownloadSdeStatus> DownloadSdeProgress { get; }

        [ObservableProperty]
        private LongProgressInfo extractSdeInfo = LongProgressInfo.Start;
        public IAccumulatingLongProgress ExtractSdeProgress { get; }

        [ObservableProperty]
        private PendingToDone parseNamesInfo = PendingToDone.Pending;
        public IProgress<PendingToDone> ParseNamesProgress { get; }

        [ObservableProperty]
        private PendingToDone parseMarketGroupsInfo = PendingToDone.Pending;
        public IProgress<PendingToDone> ParseMarketGroupsProgress { get; }

        [ObservableProperty]
        private PendingToDone parseTypesInfo = PendingToDone.Pending;
        public IProgress<PendingToDone> ParseTypesProgress { get; }

        [ObservableProperty]
        private PendingToDone parseStationOperationsInfo = PendingToDone.Pending;
        public IProgress<PendingToDone> ParseStationOperationsProgress { get; }

        [ObservableProperty]
        private PendingToDone parseNpcCorporationsInfo = PendingToDone.Pending;
        public IProgress<PendingToDone> ParseNpcCorporationsProgress { get; }

        [ObservableProperty]
        private LongProgressInfo parseRegionsInfo = LongProgressInfo.Start;
        public IAccumulatingLongProgress ParseRegionsProgress { get; }

        [ObservableProperty]
        private LongProgressInfo parseConstellationsInfo = LongProgressInfo.Start;
        public IAccumulatingLongProgress ParseConstellationsProgress { get; }

        [ObservableProperty]
        private LongProgressInfo parseSolarSystemsInfo = LongProgressInfo.Start;
        public IAccumulatingLongProgress ParseSolarSystemsProgress { get; }

        public IProgress<PendingToDone> ConvertDataProgress { get; }
        public IProgress<PendingToDone> ConvertUniverseProgress { get; }
        public IProgress<PendingToDone> CreateConnectionsProgress { get; }
        public IAccumulatingLongProgress CreateDistancesProgress { get; }
        public IAccumulatingLongProgress CopyImagesProgress { get; }
        public IProgress<PendingToDone> CreateImagesProgress { get; }
        public IAccumulatingProgress<List<FileInfo>> CreateResourcesProgress { get; }

        public AssetsProgress(IProgressCollector progressCollector)
        {
            DownloadSdeProgress = new TaskSafeProgress<DownloadSdeStatus>(x => { DownloadSdeInfo = x; });
            ExtractSdeProgress = new AccumulatingLongProgress(_ => { ExtractSdeInfo = ExtractSdeProgress!.Progress; }, 100);
            ParseNamesProgress = new TaskSafeProgress<PendingToDone>(x => { ParseNamesInfo = x; });
            ParseMarketGroupsProgress = new TaskSafeProgress<PendingToDone>(x => { ParseMarketGroupsInfo = x; });
            ParseTypesProgress = new TaskSafeProgress<PendingToDone>(x => { ParseTypesInfo = x; });
            ParseStationOperationsProgress = new TaskSafeProgress<PendingToDone>(x => { ParseStationOperationsInfo = x; });
            ParseNpcCorporationsProgress = new TaskSafeProgress<PendingToDone>(x => { ParseNpcCorporationsInfo = x; });
            ParseRegionsProgress = new AccumulatingLongProgress(_ => { ParseRegionsInfo = ParseRegionsProgress!.Progress; });
            ParseConstellationsProgress = new AccumulatingLongProgress(_ => { ParseConstellationsInfo = ParseConstellationsProgress!.Progress; });
            ParseSolarSystemsProgress = new AccumulatingLongProgress(_ => { ParseSolarSystemsInfo = ParseSolarSystemsProgress!.Progress; }, 100);
            ConvertDataProgress = new TaskSafeProgress<PendingToDone>(ReportConvertData);
            ConvertUniverseProgress = new TaskSafeProgress<PendingToDone>(ReportConvertUniverse);
            CreateConnectionsProgress = new TaskSafeProgress<PendingToDone>(ReportCreateConnections);
            CreateDistancesProgress = new AccumulatingLongProgress(ReportCreateDistances, 100);
            CopyImagesProgress = new AccumulatingLongProgress(ReportCopyImages, 100);
            CreateImagesProgress = new TaskSafeProgress<PendingToDone>(ReportCreateImages);

            CreateResourcesProgress = new AccumulatingProgress<List<FileInfo>>(ReportCreateResources, AccumulateCreateResources, [], []);

            this.progressCollector = progressCollector;
        }

        private void ReportConvertData(PendingToDone status)
            => progressCollector.ConvertData = status;

        private void ReportConvertUniverse(PendingToDone status)
            => progressCollector.ConvertUniverse = status;

        private void ReportCreateConnections(PendingToDone status)
            => progressCollector.CreateConnections = status;

        private void ReportCreateDistances(long _)
            => progressCollector.CreateDistances = CreateDistancesProgress.Progress;

        private void ReportCopyImages(long _)
            => progressCollector.CopyImages = CopyImagesProgress.Progress;

        private void ReportCreateImages(PendingToDone status)
            => progressCollector.CreateImages = status;

        private void ReportCreateResources(List<FileInfo> files)
            => progressCollector.ChangedResources = [.. files];

        private List<FileInfo> AccumulateCreateResources(List<FileInfo> current, List<FileInfo> value)
            => [.. current, .. value];
    }

    public interface IProgressCollector : INotifyPropertyChanged
    {
        public PendingToDone ConvertData { get; set; }
        public PendingToDone ConvertUniverse { get; set; }

        public PendingToDone CreateConnections { get; set; }
        public LongProgressInfo CreateDistances { get; set; }

        public LongProgressInfo CopyImages { get; set; }
        public PendingToDone CreateImages { get; set; }

        public FileInfo[] ChangedResources { get; set; }
    }

    public partial class ProgressCollector : MainThreadObservable, IProgressCollector
    {
        [ObservableProperty]
        private PendingToDone convertData = PendingToDone.Pending;

        [ObservableProperty]
        private PendingToDone convertUniverse = PendingToDone.Pending;

        [ObservableProperty]
        private PendingToDone createConnections = PendingToDone.Pending;

        [ObservableProperty]
        private LongProgressInfo createDistances = LongProgressInfo.Start;

        [ObservableProperty]
        private LongProgressInfo copyImages = LongProgressInfo.Start;

        [ObservableProperty]
        private PendingToDone createImages = PendingToDone.Pending;

        [ObservableProperty]
        private FileInfo[] changedResources = [];
    }

    public static class ProgressServices
    {
        public static IServiceCollection AddAssetsProgress(this IServiceCollection services)
        {
            services.TryAddSingleton<IAssetsProgress, AssetsProgress>();
            services.TryAddSingleton<IProgressCollector, ProgressCollector>();

            return services;
        }
    }
}
