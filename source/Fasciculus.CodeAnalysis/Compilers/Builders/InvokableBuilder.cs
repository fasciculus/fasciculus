using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public abstract class InvokableBuilder<T> : TypedSymbolBuilder<T>
        where T : notnull, InvokableSymbol<T>
    {
        public InvokableBuilder(CommentContext commentContext)
            : base(commentContext) { }

        protected void Populate(T invokable)
        {

        }
    }
}
