using Fasciculus.Eve.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;

namespace Fasciculus.Eve
{
    public class DownloadSdeProgress : IProgress<DownloadSdeMessage>
    {
        public void Report(DownloadSdeMessage value)
        {
        }
    }

    public static class ProgressServices
    {
        public static IServiceCollection AddAssetsProgress(this IServiceCollection services)
        {
            services.TryAddSingleton<IProgress<DownloadSdeMessage>, DownloadSdeProgress>();

            return services;
        }

        public static HostApplicationBuilder UseAssetsProgress(this HostApplicationBuilder builder)
        {
            builder.Services.AddAssetsProgress();

            return builder;
        }
    }
}
