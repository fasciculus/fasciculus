using CommunityToolkit.Mvvm.ComponentModel;
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

    public interface IProgressCollector : INotifyPropertyChanged
    {
        public DownloadSdeStatus DownloadSde { get; set; }
        public double ExtractSde { get; set; }

        public PendingOrDone ParseNames { get; set; }
        public PendingOrDone ParseTypes { get; set; }
    }

    public partial class ProgressCollector : ObservableObject, IProgressCollector
    {
        [ObservableProperty]
        private DownloadSdeStatus downloadSde = DownloadSdeStatus.Pending;

        [ObservableProperty]
        private double extractSde;

        [ObservableProperty]
        private PendingOrDone parseNames = PendingOrDone.Pending;

        [ObservableProperty]
        private PendingOrDone parseTypes = PendingOrDone.Pending;
    }

    public static class ProgressServices
    {
        public static IServiceCollection AddAssetsProgress(this IServiceCollection services)
        {
            services.TryAddSingleton<IProgress<DownloadSdeStatus>, DownloadSdeProgress>();
            services.TryAddKeyedSingleton<ILongProgress, ExtractSdeProgress>(ServiceKeys.ExtractSde);
            services.TryAddKeyedSingleton<IProgress<PendingOrDone>, NamesParserProgress>(ServiceKeys.NamesParser);
            services.TryAddKeyedSingleton<IProgress<PendingOrDone>, TypesParserProgress>(ServiceKeys.TypesParser);

            services.TryAddSingleton<IProgressCollector, ProgressCollector>();

            return services;
        }
    }
}
