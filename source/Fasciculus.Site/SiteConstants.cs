using NuGet.Frameworks;
using System.Collections.Generic;
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
            "Fasciculus.IO",
            "Fasciculus.Reflection",
            "Fasciculus.Text",
            "Fasciculus.Threading",
        };

        public static readonly Dictionary<string, NuGetFramework[]> PackageFrameworks = new()
        {
            { "Fasciculus.Algorithms", [NetStandard20] },
            { "Fasciculus.Core", [NetStandard20] },
            { "Fasciculus.Extensions", [NetStandard20] },
            { "Fasciculus.IO", [NetStandard20] },
            { "Fasciculus.Reflection", [NetStandard20] },
            { "Fasciculus.Text", [NetStandard20] },
            { "Fasciculus.Threading", [NetStandard20] },
        };
    }
}
