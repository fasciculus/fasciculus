using Fasciculus.CodeAnalysis.Commenting;
using Fasciculus.CodeAnalysis.Configuration;
using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.CodeAnalysis.Models;
using System;
using System.IO;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal class CompilerContext
    {
        public required ParsedProject Project { get; init; }

        public TargetFramework Framework => Project.Framework;

        public DirectoryInfo Directory => Project.Directory;

        public Uri Repository => Project.Repository;

        public required CommentContext CommentContext { get; init; }

        public required bool IncludeNonAccessible { get; init; }

        public required CodeAnalyzerDebuggers Debuggers { get; init; }
    }
}
