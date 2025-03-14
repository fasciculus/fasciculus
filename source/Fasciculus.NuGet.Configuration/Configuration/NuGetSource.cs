using NuGet.Configuration;

namespace Fasciculus.NuGet.Configuration
{
    public class NuGetSource
    {
        public PackageSource Source { get; }

        public NuGetSource(PackageSource source)
        {
            Source = source;
        }

        public static implicit operator PackageSource(NuGetSource source)
            => source.Source;
    }
}
