using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class ClassBuilder : ClassOrInterfaceBuilder<ClassSymbol>, IFieldReceiver, IConstructorReceiver
    {
        private readonly FieldList fields = [];
        private readonly ConstructorList constructors = [];

        public override ClassSymbol Build(SymbolComment comment)
        {
            ClassSymbol @class = new(Framework, Package, comment)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
            };

            Events.Apply(@class.Add);
            Properties.Apply(@class.Add);

            fields.Apply(@class.Add);
            constructors.Apply(@class.Add);

            Methods.Apply(@class.Add);

            @class.AddSource(Source);

            return @class;
        }

        public void Add(FieldSymbol field)
            => fields.AddOrMergeWith(field);

        public void Add(ConstructorSymbol constructor)
            => constructors.AddOrMergeWith(constructor);
    }
}
