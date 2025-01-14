using Fasciculus.CodeAnalysis.Compilers.Builders;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public interface ICompiler
    {
        public CommentBuilder CommentBuilder { get; }

        public void PushComment();

        public void PopComment();
    }
}
