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
        public static SettingSection GetRequiredSection(this ISettings settings, string name)
            => settings.GetSection(name) ?? throw new ArgumentException();

        public static T GetRequiredItem<T>(this SettingSection section, string name, string value)
            where T : SettingItem
            => section.GetFirstItemWithAttribute<T>(name, value) ?? throw new ArgumentException();

        public static T GetRequiredItem<T>(this ISettings settings, string section, string name, string value)
            where T : SettingItem
            => settings.GetRequiredSection(section).GetRequiredItem<T>(name, value);

        public static NuGetSource GetLocalPackageSource(this ISettings settings)
        {
            AddItem item = settings.GetRequiredItem<AddItem>("config", "key", "globalPackagesFolder");

            string? configPath = item.ConfigPath;
            string? repositoryPath = item.Value;

            if (configPath is not null && repositoryPath is not null)
            {
                FileInfo configFile = new(configPath);
                DirectoryInfo? configDirectory = configFile.Directory;

                if (configDirectory is not null)
                {
                    return new(configDirectory.Combine(repositoryPath));
                }
            }

            throw new ArgumentException();
        }

        public static NuGetSource GetLocalPackageSource(this NuGetSettings settings)
            => settings.Settings.GetLocalPackageSource();

        public static NuGetSources GetRemotePackageSources(this ISettings settings)
        {
            IEnumerable<NuGetSource> packageSources = PackageSourceProvider
                .LoadPackageSources(settings)
                .Where(x => x.IsEnabled && (x.IsHttp || x.IsHttps))
                .Select(x => new NuGetSource(x));

            return new(packageSources);
        }

        public static NuGetSources GetRemotePackageSources(this NuGetSettings settings)
            => settings.Settings.GetRemotePackageSources();
    }
}
