using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class NamespaceBuilder : TypeReceiver
    {
        public SymbolName Name { get; }

        public UriPath Link { get; }

        public TargetFramework Framework { get; }

        public string Package { get; }

        public NamespaceBuilder(SymbolName name, UriPath link, TargetFramework framework, string package)
        {
            Name = name;
            Link = link;
            Framework = framework;
            Package = package;
        }

        public NamespaceSymbol Build()
        {
            NamespaceSymbol @namespace = new(Name, Link, Framework, Package);

            classes.Apply(@namespace.AddOrMergeWith);

            return @namespace;
        }
    }
}
