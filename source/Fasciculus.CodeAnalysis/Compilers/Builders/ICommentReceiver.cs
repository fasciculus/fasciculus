using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal interface ICommentReceiver
    {
        public SymbolComment Comment { get; }
    }
}
