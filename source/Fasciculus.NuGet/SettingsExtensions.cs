using Fasciculus.IO;
using NuGet.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.NuGet
{
    public static class SettingsExtensions
    {
        public static PackageSource? GetLocalPackageSource(this ISettings settings)
        {
            SettingSection? section = settings.GetSection("config");
            AddItem? item = section?.GetFirstItemWithAttribute<AddItem>("key", "globalPackagesFolder");
            string? configPath = item?.ConfigPath;
            string? repositoryPath = item?.Value;

            if (configPath is not null && repositoryPath is not null)
            {
                FileInfo configFile = new(configPath);
                DirectoryInfo? configDirectory = configFile.Directory;
                DirectoryInfo? repositoryDirectory = configDirectory?.Combine(repositoryPath);

                return repositoryDirectory is null ? null : new PackageSource(repositoryDirectory.FullName);
            }

            return null;
        }

        public static PackageSources GetRemotePackageSources(this ISettings settings)
        {
            IEnumerable<PackageSource> packageSources = PackageSourceProvider
                .LoadPackageSources(settings)
                .Where(x => x.IsEnabled && (x.IsHttp || x.IsHttps));

            return new(packageSources);
        }
    }
}
