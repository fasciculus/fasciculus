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

        public static readonly CodeAnalyzerTestContext FasciculusCore = new()
        {
            Projects = [GetProject("Fasciculus.Core")],
            ProductionKind = SyntaxKind.OperatorDeclaration, // SyntaxKind.None,

            Packages = 1,
            Namespaces = 9,
            Enums = 0,
            Interfaces = 2,
            Classes = 32,

            Fields = 2,
            Members = 0,
            Events = 2,
            Properties = 30,

            Constructors = 28,
            Methods = 106,

            Summaries = 197,
        };

        public static readonly CodeAnalyzerTestContext FasciculusThreadingCommon = new()
        {
            Projects = [GetProject("Fasciculus.Threading.Common")],
            ProductionKind = SyntaxKind.None,

            Packages = 1,
            Namespaces = 1,
            Enums = 0,
            Interfaces = 0,
            Classes = 3,

            Fields = 0,
            Members = 0,
            Events = 0,
            Properties = 0,

            Constructors = 0,
            Methods = 29,

            Summaries = 32,
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
