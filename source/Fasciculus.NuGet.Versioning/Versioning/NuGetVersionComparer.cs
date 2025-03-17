using NuGet.Versioning;
using System.Collections.Generic;

namespace Fasciculus.NuGet.Versioning
{
    public abstract class NuGetVersionComparer : IEqualityComparer<NuGetVersion>, IComparer<NuGetVersion>
    {
        public static NuGetVersionComparer Ascending { get; } = new NuGetVersionAscendingComparer();
        public static NuGetVersionComparer Descending { get; } = new NuGetVersionDescendingComparer();

        protected static IVersionComparer SemanticVersionComparer { get; } = VersionComparer.Default;

        public bool Equals(NuGetVersion? x, NuGetVersion? y)
        {
            if (x is null)
            {
                return y is null;
            }

            if (y is null)
            {
                return false;
            }

            return SemanticVersionComparer.Equals(x, y);
        }

        public int GetHashCode(NuGetVersion? obj)
            => obj is null ? 0 : SemanticVersionComparer.GetHashCode(obj);

        public abstract int Compare(NuGetVersion? x, NuGetVersion? y);

        protected int CompareCore(NuGetVersion? x, NuGetVersion? y)
        {
            if (x is null)
            {
                return y is null ? 0 : -1;
            }
            if (y is null)
            {
                return 1;
            }

            return SemanticVersionComparer.Compare(x, y);
        }

        public class NuGetVersionAscendingComparer : NuGetVersionComparer
        {
            public override int Compare(NuGetVersion? x, NuGetVersion? y)
                => CompareCore(x, y);
        }

        public class NuGetVersionDescendingComparer : NuGetVersionComparer
        {
            public override int Compare(NuGetVersion? x, NuGetVersion? y)
                => -CompareCore(y, x);
        }
    }
}
