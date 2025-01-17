using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class ClassBuilder : CommentReceiver
    {
        public SymbolName Name { get; }

        public UriPath Link { get; }

        public TargetFramework Framework { get; }

        public string Package { get; }

        public SymbolModifiers Modifiers { get; }

        public ClassBuilder(SymbolName name, UriPath link, TargetFramework framework, string package, SymbolModifiers modifiers)
        {
            Name = name;
            Link = link;
            Framework = framework;
            Package = package;
            Modifiers = modifiers;
        }

        public ClassSymbol Build()
        {
            ClassSymbol @class = new(Name, Link, Framework, Package)
            {
                Modifiers = Modifiers,
                Comment = Comment
            };

            return @class;
        }
    }
}
