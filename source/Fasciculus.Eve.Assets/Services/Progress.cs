using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Collections;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Threading;
using Fasciculus.Utilities;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel;
using System.Diagnostics;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IAssetsProgress
    {
        public void ReportDownloadSde(DownloadSdeStatus value);

        public void StartExtractSde(long total);
        public void StopExtractSde();
        public void ReportExtractSde();
    }

    public class AssetsProgress : IAssetsProgress
    {
        private readonly IProgressCollector progressCollector;
        private readonly TaskSafeMutex mutex = new();

        private readonly Stopwatch extractSdeStopwatch = new Stopwatch();
        private long totalExtractSde;
        private long currentExtractSde;

        public AssetsProgress(IProgressCollector progressCollector)
        {
            this.progressCollector = progressCollector;
        }

        public void ReportDownloadSde(DownloadSdeStatus value)
        {
            using Locker locker = Locker.Lock(mutex);

            progressCollector.DownloadSde = value;
        }

        public void StartExtractSde(long total)
        {
            using Locker locker = Locker.Lock(mutex);

            totalExtractSde = total;
            currentExtractSde = 0;
            extractSdeStopwatch.Restart();
        }

        public void StopExtractSde()
        {
            using Locker locker = Locker.Lock(mutex);

            currentExtractSde = totalExtractSde;
            extractSdeStopwatch.Stop();

            progressCollector.ExtractSde = 1.0;
        }

        public void ReportExtractSde()
        {
            using Locker locker = Locker.Lock(mutex);

            ++currentExtractSde;

            if (extractSdeStopwatch.ElapsedMilliseconds > 100)
            {
                extractSdeStopwatch.Restart();

                progressCollector.ExtractSde = (1.0 * currentExtractSde) / totalExtractSde;
            }
        }
    }

    public class ExtractSdeProgress : LongProgress
    {
        private readonly IProgressCollector progressCollector;

        public ExtractSdeProgress(IProgressCollector progressCollector)
            : base(200)
        {
            this.progressCollector = progressCollector;
        }

        protected override void OnProgress()
        {
            progressCollector.ExtractSde = Progress;
        }
    }

    public class NamesParserProgress : TaskSafeProgress<PendingOrDone>
    {
        private readonly IProgressCollector progressCollector;

        public NamesParserProgress(IProgressCollector progressCollector)
        {
            this.progressCollector = progressCollector;
        }

        protected override void OnReport(PendingOrDone value)
        {
            progressCollector.ParseNames = value;
        }
    }

    public class TypesParserProgress : TaskSafeProgress<PendingOrDone>
    {
        private readonly IProgressCollector progressCollector;

        public TypesParserProgress(IProgressCollector progressCollector)
        {
            this.progressCollector = progressCollector;
        }

        protected override void OnReport(PendingOrDone value)
        {
            progressCollector.ParseTypes = value;
        }
    }

    public class RegionsParserProgress : LongProgress
    {
        private readonly IProgressCollector progressCollector;

        public RegionsParserProgress(IProgressCollector progressCollector)
            : base(200)
        {
            this.progressCollector = progressCollector;
        }

        protected override void OnProgress()
        {
            progressCollector.ParseRegions = Progress;
        }
    }

    public class ConstellationsParserProgress : LongProgress
    {
        private readonly IProgressCollector progressCollector;

        public ConstellationsParserProgress(IProgressCollector progressCollector)
            : base(200)
        {
            this.progressCollector = progressCollector;
        }

        protected override void OnProgress()
        {
            progressCollector.ParseConstellations = Progress;
        }
    }

    public class SolarSystemsParserProgress : LongProgress
    {
        private readonly IProgressCollector progressCollector;

        public SolarSystemsParserProgress(IProgressCollector progressCollector)
            : base(200)
        {
            this.progressCollector = progressCollector;
        }

        protected override void OnProgress()
        {
            progressCollector.ParseSolarSystems = Progress;
        }
    }

    public class ImageCopierProgress : LongProgress
    {
        private readonly IProgressCollector progressCollector;

        public ImageCopierProgress(IProgressCollector progressCollector)
            : base(200)
        {
            this.progressCollector = progressCollector;
        }

        protected override void OnProgress()
        {
            progressCollector.CopyImages = Progress;
        }
    }

    public class ResourceCreatorProgress : TaskSafeProgress<FileInfo>
    {
        private readonly IProgressCollector progressCollector;
        private readonly TaskSafeList<FileInfo> createdResources = [];

        public ResourceCreatorProgress(IProgressCollector progressCollector)
        {
            this.progressCollector = progressCollector;
        }

        protected override void OnReport(FileInfo value)
        {
            createdResources.Add(value);

            progressCollector.ChangedResources = createdResources.ToArray();
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

            services.TryAddKeyedSingleton<ILongProgress, ExtractSdeProgress>(ServiceKeys.ExtractSde);
            services.TryAddKeyedSingleton<IProgress<PendingOrDone>, NamesParserProgress>(ServiceKeys.NamesParser);
            services.TryAddKeyedSingleton<IProgress<PendingOrDone>, TypesParserProgress>(ServiceKeys.TypesParser);

            services.TryAddKeyedSingleton<ILongProgress, RegionsParserProgress>(ServiceKeys.RegionsParser);
            services.TryAddKeyedSingleton<ILongProgress, ConstellationsParserProgress>(ServiceKeys.ConstellationsParser);
            services.TryAddKeyedSingleton<ILongProgress, SolarSystemsParserProgress>(ServiceKeys.SolarSystemsParser);

            services.TryAddKeyedSingleton<ILongProgress, ImageCopierProgress>(ServiceKeys.ImageCopier);

            services.TryAddKeyedSingleton<IProgress<FileInfo>, ResourceCreatorProgress>(ServiceKeys.ResourcesCreator);

            services.TryAddSingleton<IProgressCollector, ProgressCollector>();

            return services;
        }
    }
}
