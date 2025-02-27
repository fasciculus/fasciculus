using NuGet.Frameworks;
using System.Collections.Generic;

namespace Fasciculus.NuGet.Frameworks
{
    public class TargetFrameworkComparer : IEqualityComparer<TargetFramework>, IComparer<TargetFramework>
    {
        public static TargetFrameworkComparer Instance { get; } = new();

        private TargetFrameworkComparer() { }

        public bool Equals(TargetFramework? x, TargetFramework? y)
        {
            if (x is null)
            {
                return y is null;
            }

            if (y is null)
            {
                return false;
            }

            return NuGetFrameworkFullComparer.Instance.Equals(x, y);
        }

        public int GetHashCode(TargetFramework? obj)
        {
            if (obj is null)
            {
                return 0;
            }

            return NuGetFrameworkFullComparer.Instance.GetHashCode(obj);
        }

        public int Compare(TargetFramework? x, TargetFramework? y)
        {
            if (x is null)
            {
                return y is null ? 0 : -1;
            }

            if (y is null)
            {
                return 1;
            }

            return NuGetFrameworkSorter.Instance.Compare(x, y);
        }
    }
}
