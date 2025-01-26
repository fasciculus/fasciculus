using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal abstract class TypeReceiver<T> : SymbolBuilder<T>, ITypeReceiver
        where T : notnull, Symbol<T>
    {
        protected readonly List<EnumSymbol> enums = [];
        protected readonly List<InterfaceSymbol> interfaces = [];
        protected readonly List<ClassSymbol> classes = [];

        public TypeReceiver(CommentContext commentContext)
            : base(commentContext) { }

        public void Add(EnumSymbol @enum)
        {
            enums.Add(@enum);
        }

        public void Add(InterfaceSymbol @interface)
        {
            interfaces.Add(@interface);
        }

        public void Add(ClassSymbol @class)
        {
            classes.Add(@class);
        }
    }
}
