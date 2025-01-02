using System;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiTargetFramework : IEquatable<ApiTargetFramework>, IComparable<ApiTargetFramework>
    {
        public required string Moniker { get; init; }
        public required string Version { get; init; }

        public int CompareTo(ApiTargetFramework? other)
        {
            if (other is null) return -1;

            int result = Moniker.CompareTo(other.Moniker);

            if (result == 0)
            {
                result = Version.CompareTo(other.Version);
            }

            return result;
        }

        public bool Equals(ApiTargetFramework? other)
            => other is not null && Moniker.Equals(other.Moniker) && Version.Equals(other.Version);

        public override bool Equals(object? obj)
            => Equals(obj as ApiTargetFramework);

        public override int GetHashCode()
            => Moniker.GetHashCode() ^ Version.GetHashCode();
    }
}
