using System;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiNamespace : ApiElement, IComparable<ApiNamespace>, IEquatable<ApiNamespace>
    {
        public int CompareTo(ApiNamespace? other)
            => other is null ? -1 : Name.CompareTo(other.Name);

        public bool Equals(ApiNamespace? other)
            => other is not null && Name.Equals(other.Name);

        public override bool Equals(object? obj)
            => Equals(obj as ApiTargetFramework);

        public override int GetHashCode()
            => Name.GetHashCode();
    }
}
