using NuGet.Frameworks;
using System.Collections.Generic;
using static Fasciculus.NuGet.Frameworks.MoreFrameworks;
using static NuGet.Frameworks.FrameworkConstants.CommonFrameworks;

namespace Fasciculus.Site
{
    public static class SiteConstants
    {
        public static readonly string[] PackageNames =
        {
            "Fasciculus.Algorithms",
            "Fasciculus.Core",
            "Fasciculus.Extensions",
            "Fasciculus.IO"
        };

        public static readonly Dictionary<string, NuGetFramework[]> PackageFrameworks = new()
        {
            { "Fasciculus.Algorithms", [NetStandard20, Net90] },
            { "Fasciculus.Core", [NetStandard20, Net90] },
            { "Fasciculus.Extensions", [NetStandard20, Net90] },
            { "Fasciculus.IO", [NetStandard20, Net90] },
        };
    }
}
