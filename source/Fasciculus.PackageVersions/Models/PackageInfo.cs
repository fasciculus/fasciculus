using NuGet.Versioning;

namespace Fasciculus.PackageVersions.Models
{
    public class PackageInfo
    {
        public string Name { get; }

        public NuGetVersion Version { get; }

        public PackageInfo(string name, NuGetVersion version)
        {
            Name = name;
            Version = version;
        }
    }
}
