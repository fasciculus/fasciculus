using Microsoft.CodeAnalysis;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    /// <summary>
    /// A parsed project.
    /// </summary>
    public class ParsedProject : IEnumerable<SyntaxTree>
    {
        private readonly SyntaxTree[] syntaxTrees;

        public string Name { get; }

        public ParsedProject(string name, IEnumerable<SyntaxTree> syntaxTrees)
        {
            this.syntaxTrees = [.. syntaxTrees];

            Name = name;
        }

        public ParsedProject(string name)
            : this(name, []) { }

        public IEnumerator<SyntaxTree> GetEnumerator()
            => syntaxTrees.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
