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
            progressCollector.DownloadSdeStatus = value;
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
            progressCollector.ExtractSdeProgress = Progress;
        }
    }

    public class ParseNamesProgress : TaskSafeProgress<ParseNamesMessage>
    {
        protected override void OnReport(ParseNamesMessage value)
        {
        }
    }

    public interface IProgressCollector : INotifyPropertyChanged
    {
        public DownloadSdeStatus DownloadSdeStatus { get; set; }
        public double ExtractSdeProgress { get; set; }
    }

    public partial class ProgressCollector : ObservableObject, IProgressCollector
    {
        [ObservableProperty]
        private DownloadSdeStatus downloadSdeStatus = DownloadSdeStatus.Pending;

        [ObservableProperty]
        private double extractSdeProgress;
    }

    public static class ProgressServices
    {
        public static IServiceCollection AddAssetsProgress(this IServiceCollection services)
        {
            services.TryAddSingleton<IProgress<DownloadSdeStatus>, DownloadSdeProgress>();
            services.TryAddKeyedSingleton<ILongProgress, ExtractSdeProgress>(nameof(ExtractSde));
            services.TryAddSingleton<IProgress<ParseNamesMessage>, ParseNamesProgress>();

            services.TryAddSingleton<IProgressCollector, ProgressCollector>();

            return services;
        }
    }
}
