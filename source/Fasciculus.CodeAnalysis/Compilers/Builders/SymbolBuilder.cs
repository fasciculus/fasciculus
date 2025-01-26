using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal abstract class SymbolBuilder<T> : ICommentReceiver
        where T : notnull, Symbol<T>
    {
        protected readonly CommentContext commentContext;

        public required SymbolName Name { get; init; }

        public required UriPath Link { get; init; }

        public required TargetFramework Framework { get; init; }

        public required string Package { get; init; }

        public required SymbolModifiers Modifiers { get; init; }

        public SymbolComment Comment { get; }

        public SymbolBuilder(CommentContext commentContext)
        {
            this.commentContext = commentContext;

            Comment = SymbolComment.Empty(commentContext);
        }

        public abstract T Build();
    }
}
