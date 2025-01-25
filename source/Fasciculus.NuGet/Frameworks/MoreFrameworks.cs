using NuGet.Frameworks;

namespace Fasciculus.NuGet.Frameworks
{
    public static class MoreFrameworks
    {
        public static readonly NuGetFramework Net90
            = new(FrameworkConstants.FrameworkIdentifiers.NetCoreApp, new(9, 0, 0, 0));
    }
}
