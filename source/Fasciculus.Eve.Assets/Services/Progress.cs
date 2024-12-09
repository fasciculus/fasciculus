using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Support;
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

        public WorkState ParseNamesInfo { get; }
        public IProgress<WorkState> ParseNamesProgress { get; }

        public WorkState ParseMarketGroupsInfo { get; }
        public IProgress<WorkState> ParseMarketGroupsProgress { get; }

        public WorkState ParseTypesInfo { get; }
        public IProgress<WorkState> ParseTypesProgress { get; }

        public WorkState ParseStationOperationsInfo { get; }
        public IProgress<WorkState> ParseStationOperationsProgress { get; }

        public WorkState ParseNpcCorporationsInfo { get; }
        public IProgress<WorkState> ParseNpcCorporationsProgress { get; }

        public WorkState ParsePlanetSchematicsInfo { get; }
        public IProgress<WorkState> ParsePlanetSchematicsProgress { get; }

        public LongProgressInfo ParseRegionsInfo { get; }
        public IAccumulatingLongProgress ParseRegionsProgress { get; }

        public LongProgressInfo ParseConstellationsInfo { get; }
        public IAccumulatingLongProgress ParseConstellationsProgress { get; }

        public LongProgressInfo ParseSolarSystemsInfo { get; }
        public IAccumulatingLongProgress ParseSolarSystemsProgress { get; }

        public WorkState ConvertDataInfo { get; }
        public IProgress<WorkState> ConvertDataProgress { get; }

        public WorkState ConvertUniverseInfo { get; }
        public IProgress<WorkState> ConvertUniverseProgress { get; }

        public WorkState CreateConnectionsInfo { get; }
        public IProgress<WorkState> CreateConnectionsProgress { get; }

        public LongProgressInfo CreateDistancesInfo { get; }
        public IAccumulatingLongProgress CreateDistancesProgress { get; }

        public LongProgressInfo CopyImagesInfo { get; }
        public IAccumulatingLongProgress CopyImagesProgress { get; }

        public WorkState CreateImagesInfo { get; }
        public IProgress<WorkState> CreateImagesProgress { get; }

        public FileInfo[] CreateResourcesInfo { get; }
        public IAccumulatingProgress<List<FileInfo>> CreateResourcesProgress { get; }
    }

    public partial class AssetsProgress : MainThreadObservable, IAssetsProgress
    {
        [ObservableProperty]
        private DownloadSdeStatus downloadSdeInfo = DownloadSdeStatus.Pending;
        public IProgress<DownloadSdeStatus> DownloadSdeProgress { get; }

        [ObservableProperty]
        private LongProgressInfo extractSdeInfo = LongProgressInfo.Start;
        public IAccumulatingLongProgress ExtractSdeProgress { get; }

        [ObservableProperty]
        private WorkState parseNamesInfo = WorkState.Pending;
        public IProgress<WorkState> ParseNamesProgress { get; }

        [ObservableProperty]
        private WorkState parseMarketGroupsInfo = WorkState.Pending;
        public IProgress<WorkState> ParseMarketGroupsProgress { get; }

        [ObservableProperty]
        private WorkState parseTypesInfo = WorkState.Pending;
        public IProgress<WorkState> ParseTypesProgress { get; }

        [ObservableProperty]
        private WorkState parseStationOperationsInfo = WorkState.Pending;
        public IProgress<WorkState> ParseStationOperationsProgress { get; }

        [ObservableProperty]
        private WorkState parseNpcCorporationsInfo = WorkState.Pending;
        public IProgress<WorkState> ParseNpcCorporationsProgress { get; }

        [ObservableProperty]
        private WorkState parsePlanetSchematicsInfo = WorkState.Pending;
        public IProgress<WorkState> ParsePlanetSchematicsProgress { get; }

        [ObservableProperty]
        private LongProgressInfo parseRegionsInfo = LongProgressInfo.Start;
        public IAccumulatingLongProgress ParseRegionsProgress { get; }

        [ObservableProperty]
        private LongProgressInfo parseConstellationsInfo = LongProgressInfo.Start;
        public IAccumulatingLongProgress ParseConstellationsProgress { get; }

        [ObservableProperty]
        private LongProgressInfo parseSolarSystemsInfo = LongProgressInfo.Start;
        public IAccumulatingLongProgress ParseSolarSystemsProgress { get; }

        [ObservableProperty]
        private WorkState convertDataInfo = WorkState.Pending;
        public IProgress<WorkState> ConvertDataProgress { get; }

        [ObservableProperty]
        private WorkState convertUniverseInfo = WorkState.Pending;
        public IProgress<WorkState> ConvertUniverseProgress { get; }

        [ObservableProperty]
        private WorkState createConnectionsInfo = WorkState.Pending;
        public IProgress<WorkState> CreateConnectionsProgress { get; }

        [ObservableProperty]
        private LongProgressInfo createDistancesInfo = LongProgressInfo.Start;
        public IAccumulatingLongProgress CreateDistancesProgress { get; }

        [ObservableProperty]
        private LongProgressInfo copyImagesInfo = LongProgressInfo.Start;
        public IAccumulatingLongProgress CopyImagesProgress { get; }

        [ObservableProperty]
        private WorkState createImagesInfo = WorkState.Pending;
        public IProgress<WorkState> CreateImagesProgress { get; }

        [ObservableProperty]
        private FileInfo[] createResourcesInfo = [];
        public IAccumulatingProgress<List<FileInfo>> CreateResourcesProgress { get; }

        public AssetsProgress()
        {
            DownloadSdeProgress = new TaskSafeProgress<DownloadSdeStatus>(x => { DownloadSdeInfo = x; });
            ExtractSdeProgress = new AccumulatingLongProgress(_ => { ExtractSdeInfo = ExtractSdeProgress!.Progress; });
            ParseNamesProgress = new WorkStateProgress(x => { ParseNamesInfo = x; });
            ParseMarketGroupsProgress = new WorkStateProgress(x => { ParseMarketGroupsInfo = x; });
            ParseTypesProgress = new WorkStateProgress(x => { ParseTypesInfo = x; });
            ParseStationOperationsProgress = new WorkStateProgress(x => { ParseStationOperationsInfo = x; });
            ParseNpcCorporationsProgress = new WorkStateProgress(x => { ParseNpcCorporationsInfo = x; });
            ParsePlanetSchematicsProgress = new WorkStateProgress(x => { ParsePlanetSchematicsInfo = x; });
            ParseRegionsProgress = new AccumulatingLongProgress(_ => { ParseRegionsInfo = ParseRegionsProgress!.Progress; });
            ParseConstellationsProgress = new AccumulatingLongProgress(_ => { ParseConstellationsInfo = ParseConstellationsProgress!.Progress; });
            ParseSolarSystemsProgress = new AccumulatingLongProgress(_ => { ParseSolarSystemsInfo = ParseSolarSystemsProgress!.Progress; });
            ConvertDataProgress = new WorkStateProgress(x => { ConvertDataInfo = x; });
            ConvertUniverseProgress = new WorkStateProgress(x => { ConvertUniverseInfo = x; });
            CreateConnectionsProgress = new WorkStateProgress(x => { CreateConnectionsInfo = x; });
            CreateDistancesProgress = new AccumulatingLongProgress(_ => { CreateDistancesInfo = CreateDistancesProgress!.Progress; });
            CopyImagesProgress = new AccumulatingLongProgress(_ => { CopyImagesInfo = CopyImagesProgress!.Progress; });
            CreateImagesProgress = new WorkStateProgress(x => { CreateImagesInfo = x; });

            CreateResourcesProgress
                = new AccumulatingProgress<List<FileInfo>>(x => { CreateResourcesInfo = [.. x]; }, (x, y) => [.. x, .. y], [], []);
        }
    }

    public static class ProgressServices
    {
        public static IServiceCollection AddAssetsProgress(this IServiceCollection services)
        {
            services.TryAddSingleton<IAssetsProgress, AssetsProgress>();

            return services;
        }
    }
}
