using Fasciculus.Eve.Services;
using Fasciculus.IO;
using Fasciculus.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;

namespace Fasciculus.Eve
{
    public class DownloadSdeProgress : TaskSafeProgress<DownloadSdeMessage>
    {
        protected override void Process(DownloadSdeMessage value)
        {
            ColorConsoleSnippet labelSnippet = ColorConsoleSnippet.Create("sde.zip download: ");

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

    public class ExtractSdeProgress : TaskSafeProgress<UnzipProgressMessage>
    {
        private readonly Stopwatch stopwatch = new();

        protected override void Process(UnzipProgressMessage value)
        {
            if (ProcessNeeded(value))
            {
                ColorConsoleSnippet labelSnippet = ColorConsoleSnippet.Create("sde.zip extract : ");
                ConsoleColor currentColor = value.CurrentUncompressed < value.TotalUncompressed ? ConsoleColor.Yellow : ConsoleColor.Green;
                ColorConsoleSnippet currentSnippet = ColorConsoleSnippet.Create($"{value.CurrentUncompressed}", currentColor);
                ColorConsoleSnippet totalSnippet = ColorConsoleSnippet.Create($"/{value.TotalUncompressed}");

                ColorConsole.Write(0, 1, labelSnippet, currentSnippet, totalSnippet);
            }
        }

        private bool ProcessNeeded(UnzipProgressMessage value)
        {
            if (value.ExtractedFile is null)
            {
                stopwatch.Restart();
                return true;
            }

            if (value.CurrentCompressed == value.TotalCompressed)
            {
                stopwatch.Stop();
                return true;
            }

            if (stopwatch.ElapsedMilliseconds >= 500)
            {
                stopwatch.Restart();
                return true;
            }

            return false;
        }
    }

    public class ParseNamesProgress : TaskSafeProgress<ParseNamesMessage>
    {
        protected override void Process(ParseNamesMessage value)
        {
            ColorConsoleSnippet labelSnippet = ColorConsoleSnippet.Create("Names: ");

            ColorConsoleSnippet valueSnippet = value.Status switch
            {
                ParseNamesStatus.Pending => ColorConsoleSnippet.Create("Pending", ConsoleColor.Yellow),
                ParseNamesStatus.Done => ColorConsoleSnippet.Create("Done   ", ConsoleColor.Green),
                _ => ColorConsoleSnippet.Create(""),
            };

            ColorConsole.Write(0, 2, labelSnippet, valueSnippet);
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

        public static HostApplicationBuilder UseAssetsProgress(this HostApplicationBuilder builder)
        {
            builder.Services.AddAssetsProgress();

            return builder;
        }
    }
}
