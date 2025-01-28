using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IAccessorSymbol
    {
    }

    internal class AccessorSymbol : Symbol<AccessorSymbol>, IAccessorSymbol
    {
        public AccessorSymbol(TargetFramework framework, string package, SymbolComment comment)
            : base(SymbolKind.Accessor, framework, package, comment) { }

        private AccessorSymbol(AccessorSymbol other, bool clone)
            : base(other, clone) { }

        public AccessorSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
            };
        }

        public override string? ToString()
            => $"{Name};";

        public static AccessorSymbol CreateGet(TargetFramework framework, string package, SymbolComment comment, UriPath propertyLink,
            SymbolModifiers modifiers)
        {
            return new(framework, package, comment)
            {
                Name = new("get"),
                Link = propertyLink.Append("get"),
                Modifiers = modifiers
            };
        }
    }
}
