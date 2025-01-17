using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class CompilationUnitInfo
    {
        private readonly NamespaceList namespaces = [];

        public IEnumerable<NamespaceSymbol> Namespaces => namespaces;

        public void AddOrMergeWith(NamespaceSymbol @namespace)
            => namespaces.AddOrMergeWith(@namespace);
    }
}
