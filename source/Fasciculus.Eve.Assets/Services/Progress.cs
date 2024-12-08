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

        public PendingToDone ConvertDataInfo { get; }
        public IProgress<PendingToDone> ConvertDataProgress { get; }

        public PendingToDone ConvertUniverseInfo { get; }
        public IProgress<PendingToDone> ConvertUniverseProgress { get; }

        public PendingToDone CreateConnectionsInfo { get; }
        public IProgress<PendingToDone> CreateConnectionsProgress { get; }

        public LongProgressInfo CreateDistancesInfo { get; }
        public IAccumulatingLongProgress CreateDistancesProgress { get; }

        public LongProgressInfo CopyImagesInfo { get; }
        public IAccumulatingLongProgress CopyImagesProgress { get; }

        public PendingToDone CreateImagesInfo { get; }
        public IProgress<PendingToDone> CreateImagesProgress { get; }

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

        [ObservableProperty]
        private PendingToDone convertDataInfo;
        public IProgress<PendingToDone> ConvertDataProgress { get; }

        [ObservableProperty]
        private PendingToDone convertUniverseInfo;
        public IProgress<PendingToDone> ConvertUniverseProgress { get; }

        [ObservableProperty]
        private PendingToDone createConnectionsInfo;
        public IProgress<PendingToDone> CreateConnectionsProgress { get; }

        [ObservableProperty]
        private LongProgressInfo createDistancesInfo = LongProgressInfo.Start;
        public IAccumulatingLongProgress CreateDistancesProgress { get; }

        [ObservableProperty]
        private LongProgressInfo copyImagesInfo = LongProgressInfo.Start;
        public IAccumulatingLongProgress CopyImagesProgress { get; }

        [ObservableProperty]
        private PendingToDone createImagesInfo;
        public IProgress<PendingToDone> CreateImagesProgress { get; }

        [ObservableProperty]
        private FileInfo[] createResourcesInfo = [];
        public IAccumulatingProgress<List<FileInfo>> CreateResourcesProgress { get; }

        public AssetsProgress()
        {
            DownloadSdeProgress = new TaskSafeProgress<DownloadSdeStatus>(x => { DownloadSdeInfo = x; });
            ExtractSdeProgress = new AccumulatingLongProgress(_ => { ExtractSdeInfo = ExtractSdeProgress!.Progress; });
            ParseNamesProgress = new TaskSafeProgress<PendingToDone>(x => { ParseNamesInfo = x; });
            ParseMarketGroupsProgress = new TaskSafeProgress<PendingToDone>(x => { ParseMarketGroupsInfo = x; });
            ParseTypesProgress = new TaskSafeProgress<PendingToDone>(x => { ParseTypesInfo = x; });
            ParseStationOperationsProgress = new TaskSafeProgress<PendingToDone>(x => { ParseStationOperationsInfo = x; });
            ParseNpcCorporationsProgress = new TaskSafeProgress<PendingToDone>(x => { ParseNpcCorporationsInfo = x; });
            ParseRegionsProgress = new AccumulatingLongProgress(_ => { ParseRegionsInfo = ParseRegionsProgress!.Progress; });
            ParseConstellationsProgress = new AccumulatingLongProgress(_ => { ParseConstellationsInfo = ParseConstellationsProgress!.Progress; });
            ParseSolarSystemsProgress = new AccumulatingLongProgress(_ => { ParseSolarSystemsInfo = ParseSolarSystemsProgress!.Progress; });
            ConvertDataProgress = new TaskSafeProgress<PendingToDone>(x => { ConvertDataInfo = x; });
            ConvertUniverseProgress = new TaskSafeProgress<PendingToDone>(x => { ConvertUniverseInfo = x; });
            CreateConnectionsProgress = new TaskSafeProgress<PendingToDone>(x => { CreateConnectionsInfo = x; });
            CreateDistancesProgress = new AccumulatingLongProgress(_ => { CreateDistancesInfo = CreateDistancesProgress!.Progress; });
            CopyImagesProgress = new AccumulatingLongProgress(_ => { CopyImagesInfo = CopyImagesProgress!.Progress; });
            CreateImagesProgress = new TaskSafeProgress<PendingToDone>(x => { CreateImagesInfo = x; });

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
