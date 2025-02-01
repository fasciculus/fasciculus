using Fasciculus.CodeAnalysis.Frameworking;
using Microsoft.CodeAnalysis;
using System;
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
        public required string AssemblyName { get; init; }

        public required TargetFramework Framework { get; init; }

        public required DirectoryInfo ProjectDirectory { get; init; }

        public required Uri Repository { get; init; }

        private readonly SyntaxTree[] syntaxTrees;

        public ParsedProject(IEnumerable<SyntaxTree> syntaxTrees)
        {
            this.syntaxTrees = [.. syntaxTrees];
        }

        public IEnumerator<SyntaxTree> GetEnumerator()
            => syntaxTrees.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
