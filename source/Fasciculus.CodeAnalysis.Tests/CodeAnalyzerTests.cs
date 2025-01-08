using Fasciculus.CodeAnalysis.Support;
using Fasciculus.IO;
using Fasciculus.IO.Searching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fasciculus.CodeAnalysis.Tests
{
    [TestClass]
    public class CodeAnalyzerTests : TestsBase
    {
        private static readonly SearchPath searchPath = SearchPath.WorkingDirectoryAndParents;
        private static readonly string[] projectNames = ["Fasciculus.Core", "Fasciculus.Extensions"];

        [TestMethod]
        public void Test()
        {
            CodeAnalyzerResult result = CodeAnalyzer.Create()
                .WithProjectFiles(GetProjectFiles())
                .Build().Analyze();

            ReportNodeKindReporter();
        }

        private static IEnumerable<FileInfo> GetProjectFiles()
        {
            foreach (var projectName in projectNames)
            {
                DirectoryInfo directory = DirectorySearch.Search(projectName, searchPath).First();

                yield return directory.File(projectName + ".csproj");
            }
        }

        private void ReportNodeKindReporter()
        {
            StringBuilder stringBuilder = new();
            using StringWriter writer = new(stringBuilder);

            if (NodeKindReporter.Instance.Report(writer))
            {
                Log("--- Unhandled SyntaxKind ---");
                Log(stringBuilder.ToString());
            }
        }
    }
}
