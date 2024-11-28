using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Collections;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Threading;
using Fasciculus.Utilities;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IAssetsProgress
    {
        public IAccumulatingLongProgress ExtractSdeProgress { get; }

        public void ReportDownloadSde(DownloadSdeStatus status);
        public void ReportParseNames(PendingOrDone status);
    }

    public class AssetsProgress : IAssetsProgress
    {
        private readonly IProgressCollector progressCollector;
        private readonly ReentrantTaskSafeMutex mutex = new();

        public IAccumulatingLongProgress ExtractSdeProgress { get; }

        public AssetsProgress(IProgressCollector progressCollector)
        {
            ExtractSdeProgress = new AccumulatingLongProgress(OnExtractSdeProgress, 100);

            this.progressCollector = progressCollector;
        }

        private void OnExtractSdeProgress(long _)
            => progressCollector.ExtractSde = ExtractSdeProgress.Progress;

        public void ReportDownloadSde(DownloadSdeStatus status)
            => progressCollector.DownloadSde = status;

        public void ReportParseNames(PendingOrDone status)
            => progressCollector.ParseNames = status;
    }

    public class TypesParserProgress : TaskSafeProgress<PendingOrDone>
    {
        private readonly IProgressCollector progressCollector;

        public TypesParserProgress(IProgressCollector progressCollector)
            : base(null)
        {
            this.progressCollector = progressCollector;

            report = (value) => { progressCollector.ParseTypes = value; };
        }
    }

    public class RegionsParserProgress : AccumulatingLongProgress
    {
        private readonly IProgressCollector progressCollector;

        public RegionsParserProgress(IProgressCollector progressCollector)
            : base(null, 100)
        {
            this.progressCollector = progressCollector;

            report = (_) => progressCollector.ParseRegions = Progress;
        }
    }

    public class ConstellationsParserProgress : AccumulatingLongProgress
    {
        private readonly IProgressCollector progressCollector;

        public ConstellationsParserProgress(IProgressCollector progressCollector)
            : base(null, 200)
        {
            this.progressCollector = progressCollector;

            report = (_) => progressCollector.ParseConstellations = Progress;
        }
    }

    public class SolarSystemsParserProgress : AccumulatingLongProgress
    {
        private readonly IProgressCollector progressCollector;

        public SolarSystemsParserProgress(IProgressCollector progressCollector)
            : base(null, 100)
        {
            this.progressCollector = progressCollector;

            report = (_) => progressCollector.ParseSolarSystems = Progress;
        }
    }

    public class ImageCopierProgress : AccumulatingLongProgress
    {
        private readonly IProgressCollector progressCollector;

        public ImageCopierProgress(IProgressCollector progressCollector)
            : base(null, 100)
        {
            this.progressCollector = progressCollector;

            report = (_) => progressCollector.CopyImages = Progress;
        }
    }

    public class ResourceCreatorProgress : TaskSafeProgress<FileInfo>
    {
        private readonly IProgressCollector progressCollector;
        private readonly TaskSafeList<FileInfo> createdResources = [];

        public ResourceCreatorProgress(IProgressCollector progressCollector)
        {
            this.progressCollector = progressCollector;

            report = (file) => { createdResources.Add(file); progressCollector.ChangedResources = createdResources.ToArray(); };
        }
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

            services.TryAddKeyedSingleton<IProgress<PendingOrDone>, TypesParserProgress>(ServiceKeys.TypesParser);

            services.TryAddKeyedSingleton<IAccumulatingLongProgress, RegionsParserProgress>(ServiceKeys.RegionsParser);
            services.TryAddKeyedSingleton<IAccumulatingLongProgress, ConstellationsParserProgress>(ServiceKeys.ConstellationsParser);
            services.TryAddKeyedSingleton<IAccumulatingLongProgress, SolarSystemsParserProgress>(ServiceKeys.SolarSystemsParser);

            services.TryAddKeyedSingleton<IAccumulatingLongProgress, ImageCopierProgress>(ServiceKeys.ImageCopier);

            services.TryAddKeyedSingleton<IProgress<FileInfo>, ResourceCreatorProgress>(ServiceKeys.ResourcesCreator);

            services.TryAddSingleton<IProgressCollector, ProgressCollector>();

            return services;
        }
    }
}
