using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class NamespaceBuilder
    {
        public string Name { get; }

        public TargetFramework Framework { get; }

        public string Package { get; }

        public NamespaceBuilder(string name, TargetFramework framework, string package)
        {
            Name = name;
            Framework = framework;
            Package = package;
        }

        public NamespaceSymbol Build()
        {
            SymbolName name = new(Name);
            UriPath link = new(Package, Name);

            return new(name, link, Framework, Package);
        }
    }
}
