using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal abstract class SymbolBuilder
    {
        public required SymbolName Name { get; init; }

        public required UriPath Link { get; init; }

        public required TargetFramework Framework { get; init; }

        public required string Package { get; init; }

        public required SymbolModifiers Modifiers { get; init; }

        public required UriPath Source { get; init; }
    }

    internal abstract class SymbolBuilder<T> : SymbolBuilder
        where T : notnull, Symbol<T>
    {
        public abstract T Build(SymbolComment comment);
    }
}
