using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public interface ICommentReceiver
    {
        public SymbolComment Comment { get; }
    }
}
