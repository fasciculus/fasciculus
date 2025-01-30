using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    internal abstract class InvokableBuilder<T> : TypedSymbolBuilder<T>, IParameterReceiver
        where T : notnull, InvokableSymbol<T>
    {
        private readonly ParameterList parameters = [];

        protected IEnumerable<ParameterSymbol> Parameters
            => parameters;

        public void Add(ParameterSymbol parameter)
            => parameters.Add(parameter);

        protected SymbolName CreateName()
        {
            string types = string.Join(",", parameters.Select(p => p.Type));
            string name = $"{Name.Name}({types})";
            string mangled = $"{Name.Mangled}({types})";

            return new(name);
        }

        protected UriPath CreateLink()
        {
            SymbolName[] types = [.. parameters.Select(p => p.Type)];
            string[] mangleds = [.. types.Select(p => p.Mangled)];
            string[] prefixeds = [.. mangleds.Select(m => $"-{m}")];
            string mangled = string.Join("", prefixeds);
            UriPath link = Link.Parent.Append(Link[^1] + mangled);

            return link;
        }
    }
}
