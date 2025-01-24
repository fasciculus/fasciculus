using System.Collections.Generic;

namespace Fasciculus.NuGet.Services
{
    public class IgnoredPackages : IIgnoredPackages
    {
        private readonly SortedSet<string> ignored = [];

        public IgnoredPackages()
        {
            ignored.Add("Microsoft.CSharp");
            ignored.Add("NETStandard.Library");
        }

        public bool IsIgnored(string packageId)
        {
            if (packageId.StartsWith("runtime."))
            {
                return true;
            }

            if (packageId.StartsWith("Microsoft."))
            {
                return true;
            }

            if (packageId.StartsWith("System."))
            {
                return true;
            }

            return ignored.Contains(packageId);
        }
    }
}
