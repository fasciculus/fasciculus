using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Models;
using Fasciculus.Support;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IAssetsProgress
    {
        public IProgress<DownloadSdeStatus> DownloadSde { get; }
        public IAccumulatingLongProgress ExtractSde { get; }
        public IProgress<PendingOrDone> ParseNames { get; }
        public IProgress<PendingOrDone> ParseTypes { get; }
        public IAccumulatingLongProgress RegionsParser { get; }
        public IAccumulatingLongProgress ConstellationsParser { get; }
        public IAccumulatingLongProgress SolarSystemsParser { get; }
        public IAccumulatingLongProgress ImageCopier { get; }
        public IAccumulatingProgress<List<FileInfo>> ResourceCreator { get; }
    }

    public class AssetsProgress : IAssetsProgress
    {
        private readonly IProgressCollector progressCollector;

        public IProgress<DownloadSdeStatus> DownloadSde { get; }
        public IAccumulatingLongProgress ExtractSde { get; }
        public IProgress<PendingOrDone> ParseNames { get; }
        public IProgress<PendingOrDone> ParseTypes { get; }
        public IAccumulatingLongProgress RegionsParser { get; }
        public IAccumulatingLongProgress ConstellationsParser { get; }
        public IAccumulatingLongProgress SolarSystemsParser { get; }
        public IAccumulatingLongProgress ImageCopier { get; }
        public IAccumulatingProgress<List<FileInfo>> ResourceCreator { get; }

        public AssetsProgress(IProgressCollector progressCollector)
        {
            DownloadSde = new TaskSafeProgress<DownloadSdeStatus>(ReportDownloadSde);
            ExtractSde = new AccumulatingLongProgress(ReportExtractSdeProgress, 100);
            ParseNames = new TaskSafeProgress<PendingOrDone>(ReportParseNames);
            ParseTypes = new TaskSafeProgress<PendingOrDone>(ReportParseTypes);
            RegionsParser = new AccumulatingLongProgress(ReportRegionsParserProgress, 100);
            ConstellationsParser = new AccumulatingLongProgress(ReportConstellationsParserProgress, 100);
            SolarSystemsParser = new AccumulatingLongProgress(ReportSolarSystemsParserProgress, 100);
            ImageCopier = new AccumulatingLongProgress(ReportImageCopierProgress, 100);

            ResourceCreator = new AccumulatingProgress<List<FileInfo>>(ReportResourceCreatorProgress,
                AccumulateResourceCreatorProgress, [], []);

            this.progressCollector = progressCollector;
        }

        private void ReportDownloadSde(DownloadSdeStatus status)
            => progressCollector.DownloadSde = status;

        private void ReportExtractSdeProgress(long _)
            => progressCollector.ExtractSde = ExtractSde.Progress;

        private void ReportParseNames(PendingOrDone status)
            => progressCollector.ParseNames = status;

        private void ReportParseTypes(PendingOrDone status)
            => progressCollector.ParseTypes = status;

        private void ReportRegionsParserProgress(long _)
            => progressCollector.ParseRegions = RegionsParser.Progress;

        private void ReportConstellationsParserProgress(long _)
            => progressCollector.ParseConstellations = ConstellationsParser.Progress;

        private void ReportSolarSystemsParserProgress(long _)
            => progressCollector.ParseSolarSystems = SolarSystemsParser.Progress;

        private void ReportImageCopierProgress(long _)
            => progressCollector.CopyImages = ImageCopier.Progress;

        private List<FileInfo> AccumulateResourceCreatorProgress(List<FileInfo> current, List<FileInfo> value)
            => current.Concat(value).ToList();

        private void ReportResourceCreatorProgress(List<FileInfo> files)
            => progressCollector.ChangedResources = files.ToArray();
    }

    public interface IProgressCollector : INotifyPropertyChanged
    {
        public DownloadSdeStatus DownloadSde { get; set; }
        public double ExtractSde { get; set; }

        public PendingOrDone ParseNames { get; set; }
        public PendingOrDone ParseTypes { get; set; }

        public double ParseRegions { get; set; }
        public double ParseConstellations { get; set; }
        public double ParseSolarSystems { get; set; }

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
        private PendingOrDone parseNames = PendingOrDone.Pending;

        [ObservableProperty]
        private PendingOrDone parseTypes = PendingOrDone.Pending;

        [ObservableProperty]
        private double parseRegions;

        [ObservableProperty]
        private double parseConstellations;

        [ObservableProperty]
        private double parseSolarSystems;

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
