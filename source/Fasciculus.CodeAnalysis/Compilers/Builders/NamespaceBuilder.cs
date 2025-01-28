using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class NamespaceBuilder : SymbolBuilder<NamespaceSymbol>, IEnumReceiver, IInterfaceReceiver, IClassReceiver
    {
        private readonly EnumList enums = [];
        private readonly InterfaceList interfaces = [];
        private readonly ClassList classes = [];

        public override NamespaceSymbol Build(SymbolComment comment)
        {
            NamespaceSymbol @namespace = new(Framework, Package, comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = NamespaceSymbol.NamespaceModifiers,
            };

            enums.Apply(@namespace.AddOrMergeWith);
            interfaces.Apply(@namespace.AddOrMergeWith);
            classes.Apply(@namespace.AddOrMergeWith);

            return @namespace;
        }

        public void Add(EnumSymbol @enum)
            => enums.AddOrMergeWith(@enum);

        public void Add(InterfaceSymbol @interface)
            => interfaces.AddOrMergeWith(@interface);

        public void Add(ClassSymbol @class)
            => classes.AddOrMergeWith(@class);
    }
}
