using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface IFieldSymbol : ITypedSymbol
    {

    }

    internal class FieldSymbol : TypedSymbol<FieldSymbol>, IFieldSymbol
    {
        public FieldSymbol(TargetFramework framework, string package, SymbolComment comment)
            : base(SymbolKind.Field, framework, package, comment) { }

        private FieldSymbol(FieldSymbol other, bool clone)
            : base(other, clone) { }

        public FieldSymbol Clone()
        {
            return new(this, true)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Type = Type,
            };
        }

        protected override string GetId()
            => $"{Kind}-{Name.Name}";
    }
}
