using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.IO;
using System.IO;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal class NamespaceBuilder : TypeReceiver<NamespaceSymbol>
    {
        private readonly DirectoryInfo commentsDirectory;

        public NamespaceBuilder(CommentContext commentContext, DirectoryInfo commentsDirectory)
            : base(commentContext)
        {
            this.commentsDirectory = commentsDirectory;
        }

        public override NamespaceSymbol Build()
        {
            FileInfo commentFile = commentsDirectory.File($"{Name}.xml");
            Comment.MergeWith(SymbolComment.FromFile(commentContext, commentFile));

            NamespaceSymbol @namespace = new(Framework, Package, Comment)
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
    }
}
