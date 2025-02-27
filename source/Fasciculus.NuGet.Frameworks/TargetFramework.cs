using NuGet.Frameworks;
using System;

namespace Fasciculus.NuGet.Frameworks
{
    public class TargetFramework : IEquatable<TargetFramework>, IComparable<TargetFramework>
    {
        public NuGetFramework Framework { get; }

        public TargetFramework(NuGetFramework framework)
        {
            Framework = framework;
        }

        public override int GetHashCode()
            => TargetFrameworkComparer.Instance.GetHashCode(this);

        public override bool Equals(object obj)
            => TargetFrameworkComparer.Instance.Equals(this, obj as TargetFramework);

        public override string ToString()
            => Framework.ToString();

        public bool Equals(TargetFramework? other)
            => TargetFrameworkComparer.Instance.Equals(this, other);

        public int CompareTo(TargetFramework? other)
            => TargetFrameworkComparer.Instance.Compare(this, other);

        public static bool operator ==(TargetFramework? left, TargetFramework? right)
            => TargetFrameworkComparer.Instance.Equals(left, right);

        public static bool operator !=(TargetFramework? left, TargetFramework? right)
            => !TargetFrameworkComparer.Instance.Equals(left, right);

        public static bool operator <(TargetFramework? left, TargetFramework? right)
            => TargetFrameworkComparer.Instance.Compare(left, right) < 0;

        public static bool operator >(TargetFramework? left, TargetFramework? right)
            => TargetFrameworkComparer.Instance.Compare(left, right) > 0;

        public static bool operator <=(TargetFramework? left, TargetFramework? right)
            => TargetFrameworkComparer.Instance.Compare(left, right) <= 0;

        public static bool operator >=(TargetFramework? left, TargetFramework? right)
            => TargetFrameworkComparer.Instance.Compare(left, right) >= 0;

        public static implicit operator NuGetFramework(TargetFramework targetFramework)
            => targetFramework.Framework;
    }
}
