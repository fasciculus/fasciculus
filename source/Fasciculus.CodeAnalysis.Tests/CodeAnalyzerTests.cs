using Fasciculus.CodeAnalysis.Indexing;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.CodeAnalysis.Support;
using Fasciculus.IO;
using Fasciculus.IO.Searching;
using Fasciculus.Net;
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

            LogComments(result.Indices);
            LogNodeKindReporter();
            LogCommentElementReporter();
        }

        private void LogComments(SymbolIndices indices)
        {
            UriPath path = new("Fasciculus.Core", "Fasciculus.Collections", "ObservableNotifyingEnumerable-1");
            ClassSymbol @class = indices.Classes[path];

            Log($"~~~~ {@class.Name} ~~~~");
            Log(@class.Comment.Summary);
            Log("~~~~~~~~~~~~~~~~~~~~~~~~");
        }

        private void LogNodeKindReporter()
        {
            StringBuilder stringBuilder = new();
            using StringWriter writer = new(stringBuilder);

            if (NodeKindReporter.Instance.Report(writer))
            {
                Log("--- Unhandled SyntaxKind ---");
                Log(stringBuilder.ToString());
            }
        }

        private void LogCommentElementReporter()
        {
            StringBuilder stringBuilder = new();
            using StringWriter writer = new(stringBuilder);

            if (CommentElementReporter.Instance.Report(writer))
            {
                Log("--- Unhandled comment elements ---");
                Log(stringBuilder.ToString());
            }
        }

        private static IEnumerable<FileInfo> GetProjectFiles()
        {
            foreach (var projectName in projectNames)
            {
                DirectoryInfo directory = DirectorySearch.Search(projectName, searchPath).First();

                yield return directory.File(projectName + ".csproj");
            }
        }
    }
}
