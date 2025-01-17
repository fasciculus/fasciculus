using Fasciculus.CodeAnalysis.Commenting;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public interface ICommentReceiver
    {
        public SymbolComment Comment { get; set; }
    }
}
