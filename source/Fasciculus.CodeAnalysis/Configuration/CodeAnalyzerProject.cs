using System;
using System.IO;

namespace Fasciculus.CodeAnalysis.Configuration
{
    public class CodeAnalyzerProject
    {
        public required FileInfo File { get; init; }

        public required Uri Repository { get; init; }

        public CodeAnalyzerProject() { }

        public CodeAnalyzerProject(FileInfo file, Uri repository)
        {
            File = file;
            Repository = repository;
        }
    }
}
