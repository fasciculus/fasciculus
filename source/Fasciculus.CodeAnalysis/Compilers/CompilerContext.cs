using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Configuration;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.IO;
using System.IO;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class CompilerContext
    {
        public required ParsedProject Project { get; init; }

        public TargetFramework Framework => Project.Framework;

        public DirectoryInfo ProjectDirectory => Project.ProjectDirectory;

        public DirectoryInfo CommentsDirectory
            => ProjectDirectory.Combine("Properties", "Comments");

        public required SymbolCommentContext CommentContext { get; init; }

        public required bool IncludeNonAccessible { get; init; }

        public required CodeAnalyzerDebuggers Debuggers { get; init; }
    }
}
