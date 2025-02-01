using System;
using System.IO;

namespace Fasciculus.CodeAnalysis.Configuration
{
    public class CodeAnalyzerProject
    {
        public required FileInfo ProjectFile { get; init; }

        public required Uri Repository { get; init; }

        public CodeAnalyzerProject() { }

        public CodeAnalyzerProject(FileInfo projectFile, Uri repository)
        {
            ProjectFile = projectFile;
            Repository = repository;
        }
    }
}
