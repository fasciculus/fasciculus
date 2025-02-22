using Fasciculus.IO;
using NuGet.Configuration;
using System.IO;

namespace Fasciculus.NuGet
{
    public static class SettingsExtensions
    {
        public static PackageSource? GetLocalPackageSource(this ISettings settings)
        {
            SettingSection? section = settings.GetSection("config");
            AddItem? item = section?.GetFirstItemWithAttribute<AddItem>("key", "repositoryPath");
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
    }
}
