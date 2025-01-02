using System;
using System.Collections.Generic;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiTargetFrameworkGroup : IEquatable<ApiTargetFrameworkGroup>, IComparable<ApiTargetFrameworkGroup>
    {
        public required string Name { get; init; }
        public SortedSet<ApiTargetFramework> Monikers { get; } = [];

        public int CompareTo(ApiTargetFrameworkGroup? other)
            => other is null ? -1 : Name.CompareTo(other.Name);

        public bool Equals(ApiTargetFrameworkGroup? other)
            => other is not null && Name.Equals(other.Name);

        public override bool Equals(object? obj)
            => Equals(obj as ApiTargetFrameworkGroup);

        public override int GetHashCode()
            => Name.GetHashCode();
    }
}
