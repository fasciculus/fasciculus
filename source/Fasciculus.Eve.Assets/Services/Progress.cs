using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Support;
using Fasciculus.Support;
using Fasciculus.Support.Progressing;
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

        public WorkState ParseBlueprintsInfo { get; }
        public IProgress<WorkState> ParseBlueprintsProgress { get; }

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
        public partial DownloadSdeStatus DownloadSdeInfo { get; private set; }
        public IProgress<DownloadSdeStatus> DownloadSdeProgress { get; }

        [ObservableProperty]
        public partial LongProgressInfo ExtractSdeInfo { get; private set; }
        public IAccumulatingLongProgress ExtractSdeProgress { get; }

        [ObservableProperty]
        public partial WorkState ParseNamesInfo { get; private set; }
        public IProgress<WorkState> ParseNamesProgress { get; }

        [ObservableProperty]
        public partial WorkState ParseMarketGroupsInfo { get; private set; }
        public IProgress<WorkState> ParseMarketGroupsProgress { get; }

        [ObservableProperty]
        public partial WorkState ParseTypesInfo { get; private set; }
        public IProgress<WorkState> ParseTypesProgress { get; }

        [ObservableProperty]
        public partial WorkState ParseStationOperationsInfo { get; private set; }
        public IProgress<WorkState> ParseStationOperationsProgress { get; }

        [ObservableProperty]
        public partial WorkState ParseNpcCorporationsInfo { get; private set; }
        public IProgress<WorkState> ParseNpcCorporationsProgress { get; }

        [ObservableProperty]
        public partial WorkState ParsePlanetSchematicsInfo { get; private set; }
        public IProgress<WorkState> ParsePlanetSchematicsProgress { get; }

        [ObservableProperty]
        public partial WorkState ParseBlueprintsInfo { get; private set; }
        public IProgress<WorkState> ParseBlueprintsProgress { get; }

        [ObservableProperty]
        public partial LongProgressInfo ParseRegionsInfo { get; private set; }
        public IAccumulatingLongProgress ParseRegionsProgress { get; }

        [ObservableProperty]
        public partial LongProgressInfo ParseConstellationsInfo { get; private set; }
        public IAccumulatingLongProgress ParseConstellationsProgress { get; }

        [ObservableProperty]
        public partial LongProgressInfo ParseSolarSystemsInfo { get; private set; }
        public IAccumulatingLongProgress ParseSolarSystemsProgress { get; }

        [ObservableProperty]
        public partial WorkState ConvertDataInfo { get; private set; }
        public IProgress<WorkState> ConvertDataProgress { get; }

        [ObservableProperty]
        public partial WorkState ConvertUniverseInfo { get; private set; }
        public IProgress<WorkState> ConvertUniverseProgress { get; }

        [ObservableProperty]
        public partial WorkState CreateConnectionsInfo { get; private set; }
        public IProgress<WorkState> CreateConnectionsProgress { get; }

        [ObservableProperty]
        public partial LongProgressInfo CreateDistancesInfo { get; private set; }
        public IAccumulatingLongProgress CreateDistancesProgress { get; }

        [ObservableProperty]
        public partial LongProgressInfo CopyImagesInfo { get; private set; }
        public IAccumulatingLongProgress CopyImagesProgress { get; }

        [ObservableProperty]
        public partial WorkState CreateImagesInfo { get; private set; }
        public IProgress<WorkState> CreateImagesProgress { get; }

        [ObservableProperty]
        public partial FileInfo[] CreateResourcesInfo { get; private set; }
        public IAccumulatingProgress<List<FileInfo>> CreateResourcesProgress { get; }

        public AssetsProgress()
        {
            DownloadSdeInfo = DownloadSdeStatus.Pending;
            DownloadSdeProgress = new TaskSafeProgress<DownloadSdeStatus>(x => { DownloadSdeInfo = x; });

            ExtractSdeInfo = LongProgressInfo.Start;
            ExtractSdeProgress = new AccumulatingLongProgress(_ => { ExtractSdeInfo = ExtractSdeProgress!.Progress; });

            ParseNamesInfo = WorkState.Pending;
            ParseNamesProgress = new WorkStateProgress(x => { ParseNamesInfo = x; });

            ParseMarketGroupsInfo = WorkState.Pending;
            ParseMarketGroupsProgress = new WorkStateProgress(x => { ParseMarketGroupsInfo = x; });

            ParseTypesInfo = WorkState.Pending;
            ParseTypesProgress = new WorkStateProgress(x => { ParseTypesInfo = x; });

            ParseStationOperationsInfo = WorkState.Pending;
            ParseStationOperationsProgress = new WorkStateProgress(x => { ParseStationOperationsInfo = x; });

            ParseNpcCorporationsInfo = WorkState.Pending;
            ParseNpcCorporationsProgress = new WorkStateProgress(x => { ParseNpcCorporationsInfo = x; });

            ParsePlanetSchematicsInfo = WorkState.Pending;
            ParsePlanetSchematicsProgress = new WorkStateProgress(x => { ParsePlanetSchematicsInfo = x; });

            ParseBlueprintsInfo = WorkState.Pending;
            ParseBlueprintsProgress = new WorkStateProgress(x => { ParseBlueprintsInfo = x; });

            ParseRegionsInfo = LongProgressInfo.Start;
            ParseRegionsProgress = new AccumulatingLongProgress(_ => { ParseRegionsInfo = ParseRegionsProgress!.Progress; });

            ParseConstellationsInfo = LongProgressInfo.Start;
            ParseConstellationsProgress = new AccumulatingLongProgress(_ => { ParseConstellationsInfo = ParseConstellationsProgress!.Progress; });

            ParseSolarSystemsInfo = LongProgressInfo.Start;
            ParseSolarSystemsProgress = new AccumulatingLongProgress(_ => { ParseSolarSystemsInfo = ParseSolarSystemsProgress!.Progress; });

            ConvertDataInfo = WorkState.Pending;
            ConvertDataProgress = new WorkStateProgress(x => { ConvertDataInfo = x; });

            ConvertUniverseInfo = WorkState.Pending;
            ConvertUniverseProgress = new WorkStateProgress(x => { ConvertUniverseInfo = x; });

            CreateConnectionsInfo = WorkState.Pending;
            CreateConnectionsProgress = new WorkStateProgress(x => { CreateConnectionsInfo = x; });

            CreateDistancesInfo = LongProgressInfo.Start;
            CreateDistancesProgress = new AccumulatingLongProgress(_ => { CreateDistancesInfo = CreateDistancesProgress!.Progress; });

            CopyImagesInfo = LongProgressInfo.Start;
            CopyImagesProgress = new AccumulatingLongProgress(_ => { CopyImagesInfo = CopyImagesProgress!.Progress; });

            CreateImagesInfo = WorkState.Pending;
            CreateImagesProgress = new WorkStateProgress(x => { CreateImagesInfo = x; });

            CreateResourcesInfo = [];
            CreateResourcesProgress
                = new AccumulatingProgress<List<FileInfo>>(x => { CreateResourcesInfo = [.. x]; }, (x, y) => [.. x, .. y], [], []);
        }
    }
}
