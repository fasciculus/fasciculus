using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Support
{
    public class SymbolWalker
    {
        public virtual void VisitSymbol(Symbol symbol)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.Package: VisitPackage((PackageSymbol)symbol); break;
                case SymbolKind.Namespace: VisitNamespace((NamespaceSymbol)symbol); break;
                case SymbolKind.Class: VisitClass((ClassSymbol)symbol); break;
            }
        }

        public virtual void VisitSymbols(IEnumerable<Symbol> symbols)
        {
            symbols.Apply(VisitSymbol);
        }

        public virtual void VisitPackage(PackageSymbol package)
        {
            VisitSymbols(package.Namespaces);
        }

        public virtual void VisitNamespace(NamespaceSymbol @namespace)
        {
            VisitSymbols(@namespace.Classes);
        }

        public virtual void VisitClass(ClassSymbol @class)
        {
        }
    }
}
