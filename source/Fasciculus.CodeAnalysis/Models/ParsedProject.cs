using Fasciculus.CodeAnalysis.Frameworking;
using Microsoft.CodeAnalysis;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        public DirectoryInfo? ProjectDirectory { get; }

        private readonly SyntaxTree[] syntaxTrees;

        public ParsedProject(string name, TargetFramework framework, DirectoryInfo? projectDirectory, IEnumerable<SyntaxTree> syntaxTrees)
        {
            Name = name;
            Framework = framework;
            ProjectDirectory = projectDirectory;

            this.syntaxTrees = [.. syntaxTrees];
        }

        public IEnumerator<SyntaxTree> GetEnumerator()
            => syntaxTrees.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
