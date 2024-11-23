using Fasciculus.IO;
using Fasciculus.Utilities;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public class DownloadSdeProgress : TaskSafeProgress<DownloadSdeMessage>
    {
        protected override void Process(DownloadSdeMessage value)
        {
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

    public static class ProgressServices
    {
        public static IServiceCollection AddAssetsProgress(this IServiceCollection services)
        {
            services.TryAddSingleton<IProgress<DownloadSdeMessage>, DownloadSdeProgress>();
            services.TryAddSingleton<IProgress<UnzipProgressMessage>, ExtractSdeProgress>();
            services.TryAddSingleton<IProgress<ParseNamesMessage>, ParseNamesProgress>();

            return services;
        }
    }
}
