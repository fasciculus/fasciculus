using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class PropertyBuilder : ICommentReceiver
    {
        public required SymbolName Name { get; init; }

        public required TargetFramework Framework { get; init; }

        public required string Package { get; init; }

        public required UriPath Link { get; init; }

        public required SymbolModifiers Modifiers { get; init; }

        public required AccessorList Accessors { get; init; }

        public SymbolComment Comment { get; set; } = SymbolComment.Empty;

        public PropertySymbol Build()
        {
            PropertySymbol property = new(Framework, Package)
            {
                Name = Name,
                Link = Link,
                Modifiers = Modifiers,
                Comment = Comment,
            };

            Accessors.Apply(property.Add);

            return property;
        }
    }
}
