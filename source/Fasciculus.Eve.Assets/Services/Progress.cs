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
        public DownloadSdeStatus DownloadSde { get; }
        public IProgress<DownloadSdeStatus> DownloadSdeProgress { get; }

        public IAccumulatingLongProgress ExtractSde { get; }

        public IProgress<PendingToDone> ParseNames { get; }

        public PendingToDone ParseMarketGroups { get; }
        public IProgress<PendingToDone> ParseMarketGroupsProgress { get; }

        public IProgress<PendingToDone> ParseTypes { get; }
        public IProgress<PendingToDone> ParseStationOperations { get; }
        public IProgress<PendingToDone> ParseNpcCorporations { get; }
        public IAccumulatingLongProgress ParseRegions { get; }
        public IAccumulatingLongProgress ParseConstellations { get; }
        public IAccumulatingLongProgress ParseSolarSystems { get; }
        public IProgress<PendingToDone> ConvertData { get; }
        public IProgress<PendingToDone> ConvertUniverse { get; }
        public IProgress<PendingToDone> CreateConnections { get; }
        public IAccumulatingLongProgress CreateDistances { get; }
        public IAccumulatingLongProgress CopyImages { get; }
        public IProgress<PendingToDone> CreateImages { get; }
        public IAccumulatingProgress<List<FileInfo>> CreateResources { get; }
    }

    public partial class AssetsProgress : MainThreadObservable, IAssetsProgress
    {
        private readonly IProgressCollector progressCollector;

        [ObservableProperty]
        private DownloadSdeStatus downloadSde = DownloadSdeStatus.Pending;
        public IProgress<DownloadSdeStatus> DownloadSdeProgress { get; }

        public IAccumulatingLongProgress ExtractSde { get; }
        public IProgress<PendingToDone> ParseNames { get; }

        [ObservableProperty]
        private PendingToDone parseMarketGroups = PendingToDone.Pending;
        public IProgress<PendingToDone> ParseMarketGroupsProgress { get; }

        public IProgress<PendingToDone> ParseTypes { get; }
        public IProgress<PendingToDone> ParseStationOperations { get; }
        public IProgress<PendingToDone> ParseNpcCorporations { get; }
        public IAccumulatingLongProgress ParseRegions { get; }
        public IAccumulatingLongProgress ParseConstellations { get; }
        public IAccumulatingLongProgress ParseSolarSystems { get; }
        public IProgress<PendingToDone> ConvertData { get; }
        public IProgress<PendingToDone> ConvertUniverse { get; }
        public IProgress<PendingToDone> CreateConnections { get; }
        public IAccumulatingLongProgress CreateDistances { get; }
        public IAccumulatingLongProgress CopyImages { get; }
        public IProgress<PendingToDone> CreateImages { get; }
        public IAccumulatingProgress<List<FileInfo>> CreateResources { get; }

        public AssetsProgress(IProgressCollector progressCollector)
        {
            DownloadSdeProgress = new TaskSafeProgress<DownloadSdeStatus>(x => { DownloadSde = x; });
            ExtractSde = new AccumulatingLongProgress(ReportExtractSdeProgress, 100);
            ParseNames = new TaskSafeProgress<PendingToDone>(ReportParseNames);
            ParseMarketGroupsProgress = new TaskSafeProgress<PendingToDone>(x => { ParseMarketGroups = x; });
            ParseTypes = new TaskSafeProgress<PendingToDone>(ReportParseTypes);
            ParseStationOperations = new TaskSafeProgress<PendingToDone>(ReportParseStationOperations);
            ParseNpcCorporations = new TaskSafeProgress<PendingToDone>(ReportParseNpcCorporations);
            ParseRegions = new AccumulatingLongProgress(ReportParseRegions);
            ParseConstellations = new AccumulatingLongProgress(ReportParseConstellations);
            ParseSolarSystems = new AccumulatingLongProgress(ReportParseSolarSystems, 100);
            ConvertData = new TaskSafeProgress<PendingToDone>(ReportConvertData);
            ConvertUniverse = new TaskSafeProgress<PendingToDone>(ReportConvertUniverse);
            CreateConnections = new TaskSafeProgress<PendingToDone>(ReportCreateConnections);
            CreateDistances = new AccumulatingLongProgress(ReportCreateDistances, 100);
            CopyImages = new AccumulatingLongProgress(ReportCopyImages, 100);
            CreateImages = new TaskSafeProgress<PendingToDone>(ReportCreateImages);

            CreateResources = new AccumulatingProgress<List<FileInfo>>(ReportCreateResources, AccumulateCreateResources, [], []);

            this.progressCollector = progressCollector;
        }

        private void ReportExtractSdeProgress(long _)
            => progressCollector.ExtractSde = ExtractSde.Progress;

        private void ReportParseNames(PendingToDone status)
            => progressCollector.ParseNames = status;

        private void ReportParseTypes(PendingToDone status)
            => progressCollector.ParseTypes = status;

        private void ReportParseStationOperations(PendingToDone status)
            => progressCollector.ParseStationOperations = status;

        private void ReportParseNpcCorporations(PendingToDone status)
            => progressCollector.ParseNpcCorporations = status;

        private void ReportParseRegions(long _)
            => progressCollector.ParseRegions = ParseRegions.Progress;

        private void ReportParseConstellations(long _)
            => progressCollector.ParseConstellations = ParseConstellations.Progress;

        private void ReportParseSolarSystems(long _)
            => progressCollector.ParseSolarSystems = ParseSolarSystems.Progress;

        private void ReportConvertData(PendingToDone status)
            => progressCollector.ConvertData = status;

        private void ReportConvertUniverse(PendingToDone status)
            => progressCollector.ConvertUniverse = status;

        private void ReportCreateConnections(PendingToDone status)
            => progressCollector.CreateConnections = status;

        private void ReportCreateDistances(long _)
            => progressCollector.CreateDistances = CreateDistances.Progress;

        private void ReportCopyImages(long _)
            => progressCollector.CopyImages = CopyImages.Progress;

        private void ReportCreateImages(PendingToDone status)
            => progressCollector.CreateImages = status;

        private void ReportCreateResources(List<FileInfo> files)
            => progressCollector.ChangedResources = [.. files];

        private List<FileInfo> AccumulateCreateResources(List<FileInfo> current, List<FileInfo> value)
            => [.. current, .. value];
    }

    public interface IProgressCollector : INotifyPropertyChanged
    {
        public LongProgressInfo ExtractSde { get; set; }

        public PendingToDone ParseNames { get; set; }
        public PendingToDone ParseTypes { get; set; }
        public PendingToDone ParseStationOperations { get; set; }
        public PendingToDone ParseNpcCorporations { get; set; }

        public LongProgressInfo ParseRegions { get; set; }
        public LongProgressInfo ParseConstellations { get; set; }
        public LongProgressInfo ParseSolarSystems { get; set; }

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
        private LongProgressInfo extractSde = LongProgressInfo.Start;

        [ObservableProperty]
        private PendingToDone parseNames = PendingToDone.Pending;

        [ObservableProperty]
        private PendingToDone parseTypes = PendingToDone.Pending;

        [ObservableProperty]
        private PendingToDone parseStationOperations = PendingToDone.Pending;

        [ObservableProperty]
        private PendingToDone parseNpcCorporations = PendingToDone.Pending;

        [ObservableProperty]
        private LongProgressInfo parseRegions = LongProgressInfo.Start;

        [ObservableProperty]
        private LongProgressInfo parseConstellations = LongProgressInfo.Start;

        [ObservableProperty]
        private LongProgressInfo parseSolarSystems = LongProgressInfo.Start;

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
