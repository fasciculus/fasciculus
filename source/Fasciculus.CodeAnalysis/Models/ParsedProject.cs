using Fasciculus.CodeAnalysis.Frameworking;
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
        public string Name { get; }

        public TargetFramework Framework { get; }

        private readonly SyntaxTree[] syntaxTrees;

        public ParsedProject(string name, TargetFramework framework, IEnumerable<SyntaxTree> syntaxTrees)
        {
            Name = name;
            Framework = framework;

            this.syntaxTrees = [.. syntaxTrees];
        }

        public ParsedProject(string name, TargetFramework framework)
            : this(name, framework, []) { }

        public IEnumerator<SyntaxTree> GetEnumerator()
            => syntaxTrees.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
