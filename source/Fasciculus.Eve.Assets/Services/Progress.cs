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
        public IProgress<PendingToDone> ConvertUniverse { get; }
        public IAccumulatingLongProgress CopyImages { get; }
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
        public IProgress<PendingToDone> ConvertUniverse { get; }
        public IAccumulatingLongProgress CopyImages { get; }
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
            ConvertUniverse = new TaskSafeProgress<PendingToDone>(ReportConvertUniverse);
            CopyImages = new AccumulatingLongProgress(ReportCopyImages, 100);

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
        {
            progressCollector.ParseRegions = ParseRegions.Progress;
            progressCollector.ParseRegionsDone = ParseRegions.Done;
        }

        private void ReportParseConstellations(long _)
        {
            progressCollector.ParseConstellations = ParseConstellations.Progress;
            progressCollector.ParseConstellationsDone = ParseConstellations.Done;
        }

        private void ReportParseSolarSystems(long _)
        {
            progressCollector.ParseSolarSystems = ParseSolarSystems.Progress;
            progressCollector.ParseSolarSystemsDone = ParseSolarSystems.Done;
        }

        private void ReportConvertUniverse(PendingToDone status)
            => progressCollector.ConvertUniverse = status;

        private void ReportCopyImages(long _)
            => progressCollector.CopyImages = CopyImages.Progress;

        private void ReportCreateResources(List<FileInfo> files)
            => progressCollector.ChangedResources = files.ToArray();

        private List<FileInfo> AccumulateCreateResources(List<FileInfo> current, List<FileInfo> value)
            => current.Concat(value).ToList();
    }

    public interface IProgressCollector : INotifyPropertyChanged
    {
        public DownloadSdeStatus DownloadSde { get; set; }
        public double ExtractSde { get; set; }

        public PendingToDone ParseNames { get; set; }
        public PendingToDone ParseTypes { get; set; }

        public double ParseRegions { get; set; }
        public bool ParseRegionsDone { get; set; }

        public double ParseConstellations { get; set; }
        public bool ParseConstellationsDone { get; set; }

        public double ParseSolarSystems { get; set; }
        public bool ParseSolarSystemsDone { get; set; }

        public PendingToDone ConvertUniverse { get; set; }

        public double CopyImages { get; set; }

        public FileInfo[] ChangedResources { get; set; }
    }

    public partial class ProgressCollector : MainThreadObservable, IProgressCollector
    {
        [ObservableProperty]
        private DownloadSdeStatus downloadSde = DownloadSdeStatus.Pending;

        [ObservableProperty]
        private double extractSde;

        [ObservableProperty]
        private PendingToDone parseNames = PendingToDone.Pending;

        [ObservableProperty]
        private PendingToDone parseTypes = PendingToDone.Pending;

        [ObservableProperty]
        private double parseRegions;

        [ObservableProperty]
        private bool parseRegionsDone;

        [ObservableProperty]
        private double parseConstellations;

        [ObservableProperty]
        private bool parseConstellationsDone;

        [ObservableProperty]
        private double parseSolarSystems;

        [ObservableProperty]
        private bool parseSolarSystemsDone;

        [ObservableProperty]
        private PendingToDone convertUniverse = PendingToDone.Pending;

        [ObservableProperty]
        private double copyImages;

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
