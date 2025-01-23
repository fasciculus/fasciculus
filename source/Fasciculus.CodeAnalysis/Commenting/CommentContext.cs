namespace Fasciculus.CodeAnalysis.Commenting
{
    public class CommentContext
    {
        public required ICommentMerger Merger { get; init; }

        public required ICommentResolver Resolver { get; init; }

        public required ICommentFormatter Formatter { get; init; }
    }
}
