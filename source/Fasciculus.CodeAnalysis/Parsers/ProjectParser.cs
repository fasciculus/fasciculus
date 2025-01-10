using Fasciculus.CodeAnalysis.Extensions;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Parsers
{
    /// <summary>
    /// Parser for a single project.
    /// </summary>
    public class ProjectParser
    {
        public ParsedProject Parse(Project project, TargetFramework framework, bool includeGenerated)
        {
            IEnumerable<SyntaxTree> syntaxTrees = [];
            DirectoryInfo? directory = project.GetDirectory();

            if (Tasks.Wait(project.GetCompilationAsync()) is CSharpCompilation compilation)
            {
                syntaxTrees = compilation.SyntaxTrees.Where(t => CheckGenerated(t, includeGenerated));
            }

            return new(project.AssemblyName, framework, directory, syntaxTrees);
        }

        private static bool CheckGenerated(SyntaxTree syntaxTree, bool includeGenerated)
            => includeGenerated || !syntaxTree.IsGenerated();
    }
}
