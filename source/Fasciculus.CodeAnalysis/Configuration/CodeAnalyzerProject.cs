using Fasciculus.Net.Navigating;
using System.IO;

namespace Fasciculus.CodeAnalysis.Configuration
{
    public class CodeAnalyzerProject
    {
        public required FileInfo ProjectFile { get; init; }

        public required UriPath RepositoryDirectory { get; init; }

        public CodeAnalyzerProject() { }

        public CodeAnalyzerProject(FileInfo projectFile, UriPath repositoryDirectory)
        {
            ProjectFile = projectFile;
            RepositoryDirectory = repositoryDirectory;
        }
    }
}
