using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Models;
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

    public interface IAssetsProgress
    {
        public IProgress<DownloadSdeStatus> DownloadSde { get; }
        public IAccumulatingLongProgress ExtractSde { get; }
        public IProgress<PendingToDone> ParseNames { get; }
        public IProgress<PendingToDone> ParseTypes { get; }
        public IAccumulatingLongProgress ParseRegions { get; }
        public IAccumulatingLongProgress ParseConstellations { get; }
        public IAccumulatingLongProgress ParseSolarSystems { get; }
        public IProgress<PendingToDone> ConvertData { get; }
        public IProgress<PendingToDone> ConvertUniverse { get; }
        public IAccumulatingLongProgress CopyImages { get; }
        public IProgress<PendingToDone> CreateImages { get; }
        public IAccumulatingProgress<List<FileInfo>> CreateResources { get; }
    }

    public class AssetsProgress : IAssetsProgress
    {
        private readonly IProgressCollector progressCollector;

        public IProgress<DownloadSdeStatus> DownloadSde { get; }
        public IAccumulatingLongProgress ExtractSde { get; }
        public IProgress<PendingToDone> ParseNames { get; }
        public IProgress<PendingToDone> ParseTypes { get; }
        public IAccumulatingLongProgress ParseRegions { get; }
        public IAccumulatingLongProgress ParseConstellations { get; }
        public IAccumulatingLongProgress ParseSolarSystems { get; }
        public IProgress<PendingToDone> ConvertData { get; }
        public IProgress<PendingToDone> ConvertUniverse { get; }
        public IAccumulatingLongProgress CopyImages { get; }
        public IProgress<PendingToDone> CreateImages { get; }
        public IAccumulatingProgress<List<FileInfo>> CreateResources { get; }

        public AssetsProgress(IProgressCollector progressCollector)
        {
            DownloadSde = new TaskSafeProgress<DownloadSdeStatus>(ReportDownloadSde);
            ExtractSde = new AccumulatingLongProgress(ReportExtractSdeProgress, 100);
            ParseNames = new TaskSafeProgress<PendingToDone>(ReportParseNames);
            ParseTypes = new TaskSafeProgress<PendingToDone>(ReportParseTypes);
            ParseRegions = new AccumulatingLongProgress(ReportParseRegions, 100);
            ParseConstellations = new AccumulatingLongProgress(ReportParseConstellations, 100);
            ParseSolarSystems = new AccumulatingLongProgress(ReportParseSolarSystems, 100);
            ConvertData = new TaskSafeProgress<PendingToDone>(ReportConvertData);
            ConvertUniverse = new TaskSafeProgress<PendingToDone>(ReportConvertUniverse);
            CopyImages = new AccumulatingLongProgress(ReportCopyImages, 100);
            CreateImages = new TaskSafeProgress<PendingToDone>(ReportCreateImages);

            CreateResources = new AccumulatingProgress<List<FileInfo>>(ReportCreateResources, AccumulateCreateResources, [], []);

            this.progressCollector = progressCollector;
        }

        private void ReportDownloadSde(DownloadSdeStatus status)
            => progressCollector.DownloadSde = status;

        private void ReportExtractSdeProgress(long _)
            => progressCollector.ExtractSde = ExtractSde.Progress;

        private void ReportParseNames(PendingToDone status)
            => progressCollector.ParseNames = status;

        private void ReportParseTypes(PendingToDone status)
            => progressCollector.ParseTypes = status;

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
        public DownloadSdeStatus DownloadSde { get; set; }
        public LongProgressInfo ExtractSde { get; set; }

        public PendingToDone ParseNames { get; set; }
        public PendingToDone ParseTypes { get; set; }

        public LongProgressInfo ParseRegions { get; set; }
        public LongProgressInfo ParseConstellations { get; set; }
        public LongProgressInfo ParseSolarSystems { get; set; }

        public PendingToDone ConvertData { get; set; }
        public PendingToDone ConvertUniverse { get; set; }

        public LongProgressInfo CopyImages { get; set; }
        public PendingToDone CreateImages { get; set; }

        public FileInfo[] ChangedResources { get; set; }
    }

    public partial class ProgressCollector : MainThreadObservable, IProgressCollector
    {
        [ObservableProperty]
        private DownloadSdeStatus downloadSde = DownloadSdeStatus.Pending;

        [ObservableProperty]
        private LongProgressInfo extractSde = LongProgressInfo.Start;

        [ObservableProperty]
        private PendingToDone parseNames = PendingToDone.Pending;

        [ObservableProperty]
        private PendingToDone parseTypes = PendingToDone.Pending;

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
