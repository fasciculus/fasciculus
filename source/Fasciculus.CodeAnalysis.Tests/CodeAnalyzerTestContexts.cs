using Fasciculus.CodeAnalysis.Configuration;
using Fasciculus.IO;
using Fasciculus.IO.Searching;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Tests
{
    public static class CodeAnalyzerTestContexts
    {
        public static readonly CodeAnalyzerTestContext FasciculusAlgorithms = new()
        {
            Projects = [GetProject("Fasciculus.Algorithms")],
            ProductionKind = SyntaxKind.Parameter,

            Packages = 1,
            Namespaces = 4,
            Enums = 0,
            Interfaces = 0,
            Classes = 7,

            Fields = 0,
            Members = 0,
            Events = 0,
            Properties = 4,

            Constructors = 1,
            Methods = 17,

            Summaries = 29,
        };

        private static CodeAnalyzerProject GetProject(string name)
        {
            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents();
            DirectoryInfo directory = DirectorySearch.Search(name, searchPath).First();

            return new()
            {
                File = directory.File(name + ".csproj"),
                Repository = new($"https://github.com/fasciculus/fasciculus/tree/main/source/{name}/"),
            };
        }
    }
}
