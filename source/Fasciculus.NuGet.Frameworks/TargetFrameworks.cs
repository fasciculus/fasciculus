using NuGet.Frameworks;
using System.Collections.Generic;
using static NuGet.Frameworks.FrameworkConstants;

namespace Fasciculus.NuGet.Frameworks
{
    public static class TargetFrameworks
    {
        public static TargetFramework Unsupported { get; } = new(NuGetFramework.UnsupportedFramework);

        public static TargetFramework NetStandard_1_0 { get; } = new(CommonFrameworks.NetStandard10);

        public static TargetFramework NetStandard_1_1 { get; } = new(CommonFrameworks.NetStandard11);

        public static TargetFramework NetStandard_1_2 { get; } = new(CommonFrameworks.NetStandard12);

        public static TargetFramework NetStandard_1_3 { get; } = new(CommonFrameworks.NetStandard13);

        public static TargetFramework NetStandard_1_4 { get; } = new(CommonFrameworks.NetStandard14);

        public static TargetFramework NetStandard_1_5 { get; } = new(CommonFrameworks.NetStandard15);

        public static TargetFramework NetStandard_1_6 { get; } = new(CommonFrameworks.NetStandard16);

        public static TargetFramework NetStandard_1_7 { get; } = new(CommonFrameworks.NetStandard17);

        public static TargetFramework NetStandard_2_0 { get; } = new(CommonFrameworks.NetStandard20);

        public static TargetFramework NetStandard_2_1 { get; } = new(CommonFrameworks.NetStandard21);

        public static TargetFramework FromDirectives(IEnumerable<string> directives)
        {
            SortedSet<string> sortedDirectives = [.. directives];

            return FromDirectives(sortedDirectives);
        }

        private static TargetFramework FromDirectives(SortedSet<string> directives)
        {
            if (directives.Contains("NETSTANDARD2_1")) return NetStandard_2_1;
            if (directives.Contains("NETSTANDARD2_0")) return NetStandard_2_0;
            if (directives.Contains("NETSTANDARD1_7")) return NetStandard_1_7;
            if (directives.Contains("NETSTANDARD1_6")) return NetStandard_1_6;
            if (directives.Contains("NETSTANDARD1_5")) return NetStandard_1_5;
            if (directives.Contains("NETSTANDARD1_4")) return NetStandard_1_4;
            if (directives.Contains("NETSTANDARD1_3")) return NetStandard_1_3;
            if (directives.Contains("NETSTANDARD1_2")) return NetStandard_1_2;
            if (directives.Contains("NETSTANDARD1_1")) return NetStandard_1_1;
            if (directives.Contains("NETSTANDARD1_0")) return NetStandard_1_0;

            return Unsupported;
        }
    }
}
