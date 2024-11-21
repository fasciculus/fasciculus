using Fasciculus.Eve.Services;
using Fasciculus.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;

namespace Fasciculus.Eve
{
    public class DownloadSdeProgress : TaskSafeProgress<DownloadSdeMessage>
    {
        protected override void Process(DownloadSdeMessage value)
        {
            ColorConsoleSnippet labelSnippet = ColorConsoleSnippet.Create("sde.zip ");

            ColorConsoleSnippet valueSnippet = value.Status switch
            {
                DownloadSdeStatus.Downloading => ColorConsoleSnippet.Create("Downloading", ConsoleColor.Yellow),
                DownloadSdeStatus.NotModified => ColorConsoleSnippet.Create("Not Modified", ConsoleColor.Green),
                DownloadSdeStatus.Downloaded => ColorConsoleSnippet.Create("Downloaded ", ConsoleColor.Green),
                _ => ColorConsoleSnippet.Create(""),
            };

            ColorConsole.Write(0, 0, labelSnippet, valueSnippet);
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
