using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public abstract class TypedSymbolBuilder<T> : SymbolBuilder<T>
        where T : notnull, TypedSymbol<T>
    {
        public required string Type { get; init; }

        public TypedSymbolBuilder(CommentContext commentContext)
            : base(commentContext) { }
    }
}
