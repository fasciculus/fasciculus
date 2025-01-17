using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class TypeBuilder : TypeReceiver, ICommentReceiver
    {
        public SymbolName Name { get; }

        public TargetFramework Framework { get; }

        public string Package { get; }

        public SymbolModifiers Modifiers { get; }

        public SymbolComment Comment { get; set; } = SymbolComment.Empty;

        public TypeBuilder(SymbolName name, UriPath link, TargetFramework framework, string package, SymbolModifiers modifiers)
            : base(link)
        {
            Name = name;
            Framework = framework;
            Package = package;
            Modifiers = modifiers;
        }
    }
}
