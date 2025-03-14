using Fasciculus.IO;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.NuGet.Configuration
{
    public static class SettingsExtensions
    {
        public static NuGetSource GetLocalPackageSource(this ISettings settings)
        {
            NuGetSource? source = null;
            SettingSection? section = settings.GetSection("config");

            if (section is not null)
            {
                AddItem? item = section?.GetFirstItemWithAttribute<AddItem>("key", "globalPackagesFolder");

                if (item is not null)
                {
                    string? configPath = item.ConfigPath;
                    string? repositoryPath = item.Value;

                    if (configPath is not null && repositoryPath is not null)
                    {
                        FileInfo configFile = new(configPath);
                        DirectoryInfo? configDirectory = configFile.Directory;

                        if (configDirectory is not null)
                        {
                            DirectoryInfo repositoryDirectory = configDirectory.Combine(repositoryPath);

                            source = new(new PackageSource(repositoryDirectory.FullName));
                        }
                    }
                }
            }

            return source ?? throw new ArgumentException();
        }

        public static NuGetSources GetRemotePackageSources(this ISettings settings)
        {
            IEnumerable<NuGetSource> packageSources = PackageSourceProvider
                .LoadPackageSources(settings)
                .Where(x => x.IsEnabled && (x.IsHttp || x.IsHttps))
                .Select(x => new NuGetSource(x));

            return new(packageSources);
        }
    }
}
