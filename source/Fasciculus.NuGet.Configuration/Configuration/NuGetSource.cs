using Fasciculus.Threading;
using NuGet.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Configuration
{
    public class NuGetSource
    {
        public static AsyncLazy<NuGetSource> Global { get; } = new(GetGlobal);

        private static async Task<NuGetSource> GetGlobal()
        {
            NuGetSettings settings = await NuGetSettings.Default;

            return settings.GetGlobalSource();
        }

        public PackageSource Source { get; }

        public NuGetSource(PackageSource source)
        {
            Source = source;
        }

        public NuGetSource(DirectoryInfo directory)
            : this(new PackageSource(directory.FullName)) { }

        public static implicit operator PackageSource(NuGetSource source)
            => source.Source;
    }
}
