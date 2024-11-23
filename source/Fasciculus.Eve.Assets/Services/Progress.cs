using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.IO;
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

        protected override void Process(DownloadSdeStatus value)
        {
            progressCollector.DownloadSdeStatus = value;
        }
    }

    public class ExtractSdeProgress : TaskSafeProgress<UnzipProgressMessage>
    {
        protected override void Process(UnzipProgressMessage value)
        {
        }
    }

    public class ParseNamesProgress : TaskSafeProgress<ParseNamesMessage>
    {
        protected override void Process(ParseNamesMessage value)
        {
        }
    }

    public interface IProgressCollector : INotifyPropertyChanged
    {
        public DownloadSdeStatus DownloadSdeStatus { get; set; }
    }

    public partial class ProgressCollector : ObservableObject, IProgressCollector
    {
        [ObservableProperty]
        private DownloadSdeStatus downloadSdeStatus = DownloadSdeStatus.Pending;
    }

    public static class ProgressServices
    {
        public static IServiceCollection AddAssetsProgress(this IServiceCollection services)
        {
            services.TryAddSingleton<IProgress<DownloadSdeStatus>, DownloadSdeProgress>();
            services.TryAddSingleton<IProgress<UnzipProgressMessage>, ExtractSdeProgress>();
            services.TryAddSingleton<IProgress<ParseNamesMessage>, ParseNamesProgress>();

            services.TryAddSingleton<IProgressCollector, ProgressCollector>();

            return services;
        }
    }
}
