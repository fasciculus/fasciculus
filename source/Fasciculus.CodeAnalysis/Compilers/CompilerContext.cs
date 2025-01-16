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

        public TargetFrameworks Frameworks => new([Framework]);

        public DirectoryInfo? ProjectDirectory => Project.ProjectDirectory;

        public DirectoryInfo? CommentsDirectory
            => ProjectDirectory?.Combine("Properties", "Comments");

        public CodeAnalyzerDebuggers Debuggers { get; }

        public CompilerContext(ParsedProject project, CodeAnalyzerDebuggers debuggers)
        {
            Project = project;
            Debuggers = debuggers;
        }
    }
}
