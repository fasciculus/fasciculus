using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.IO;
using System.IO;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class CompilerContext
    {
        public ParsedProject Project { get; }

        public TargetFramework Framework => Project.Framework;

        public DirectoryInfo? ProjectDirectory => Project.ProjectDirectory;

        public DirectoryInfo? CommentsDirectory
            => ProjectDirectory?.Combine("Properties", "Comments");

        public bool IncludeNonAccessible { get; }

        public CodeAnalyzerDebuggers Debuggers { get; }

        public CompilerContext(ParsedProject project, bool includeNonAccessible, CodeAnalyzerDebuggers debuggers)
        {
            Project = project;
            IncludeNonAccessible = includeNonAccessible;
            Debuggers = debuggers;
        }
    }
}
