using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Utilities;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel;

namespace Fasciculus.Eve.Assets.Services
{
    public class DownloadSdeProgress : TaskSafeProgress<DownloadSdeStatus>
    {
        private readonly IProgressCollector progressCollector;

        public DownloadSdeProgress(IProgressCollector progressCollector)
        {
            this.progressCollector = progressCollector;
        }

        protected override void OnReport(DownloadSdeStatus value)
        {
            progressCollector.DownloadSde = value;
        }
    }

    public class ExtractSdeProgress : LongProgress
    {
        private readonly IProgressCollector progressCollector;

        public ExtractSdeProgress(IProgressCollector progressCollector)
            : base(500)
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
            : base(500)
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
            : base(500)
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
            : base(500)
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
            : base(500)
        {
            this.progressCollector = progressCollector;
        }

        protected override void OnProgress()
        {
            progressCollector.CopyImages = Progress;
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
    }

    public static class ProgressServices
    {
        public static IServiceCollection AddAssetsProgress(this IServiceCollection services)
        {
            services.TryAddSingleton<IProgress<DownloadSdeStatus>, DownloadSdeProgress>();
            services.TryAddKeyedSingleton<ILongProgress, ExtractSdeProgress>(ServiceKeys.ExtractSde);
            services.TryAddKeyedSingleton<IProgress<PendingOrDone>, NamesParserProgress>(ServiceKeys.NamesParser);
            services.TryAddKeyedSingleton<IProgress<PendingOrDone>, TypesParserProgress>(ServiceKeys.TypesParser);

            services.TryAddKeyedSingleton<ILongProgress, RegionsParserProgress>(ServiceKeys.RegionsParser);
            services.TryAddKeyedSingleton<ILongProgress, ConstellationsParserProgress>(ServiceKeys.ConstellationsParser);
            services.TryAddKeyedSingleton<ILongProgress, SolarSystemsParserProgress>(ServiceKeys.SolarSystemsParser);

            services.TryAddKeyedSingleton<ILongProgress, ImageCopierProgress>(ServiceKeys.ImageCopier);

            services.TryAddSingleton<IProgressCollector, ProgressCollector>();

            return services;
        }
    }
}
