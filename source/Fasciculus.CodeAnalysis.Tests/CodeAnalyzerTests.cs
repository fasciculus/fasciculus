using Fasciculus.CodeAnalysis.Compilers;
using Fasciculus.CodeAnalysis.Indexing;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.IO;
using Fasciculus.IO.Searching;
using Fasciculus.Net;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

            LogUnhandledSymbols();
            LogUnhandledModifiers();
            LogUnhandledCommentElements();
            LogComments(result.Indices);
        }

        private void LogComments(SymbolIndices indices)
        {
            UriPath path = new("Fasciculus.Core", "Fasciculus.Collections", "ObservableNotifyingEnumerable-1");
            ClassSymbol @class = indices.Classes[path];

            Log($"~~~~ {@class.Name} ~~~~");
            Log(@class.Comment.Summary);
            Log("~~~~~~~~~~~~~~~~~~~~~~~~");
        }

        private void LogUnhandledSymbols()
        {
            Dictionary<string, SortedSet<SyntaxKind>> unhandled = UnhandledSymbols.Instance.Unhandled();

            if (unhandled.Count > 0)
            {
                Log("--- unhandled symbols ---");

                foreach (var entry in unhandled)
                {
                    Log(entry.Key);
                    Log(string.Join(Environment.NewLine, entry.Value.Select(u => "- " + u)));
                }
            }
        }

        private void LogUnhandledModifiers()
        {
            SortedSet<string> unhandled = UnhandledModifiers.Instance.Unhandled();

            if (unhandled.Count > 0)
            {
                Log("--- unhandled modifiers ---");
                Log(string.Join(Environment.NewLine, unhandled.Select(u => "- " + u)));
            }
        }

        private void LogUnhandledCommentElements()
        {
            SortedSet<string> unhandled = UnhandledCommentElements.Instance.Unhandled();

            if (unhandled.Count > 0)
            {
                Log("--- unhandled comment elements ---");
                Log(string.Join(Environment.NewLine, unhandled.Select(u => "- " + u)));
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
