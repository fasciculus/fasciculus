using Fasciculus.CodeAnalysis.Compilers;
using Fasciculus.CodeAnalysis.Configuration;
using Fasciculus.CodeAnalysis.Debugging;
using Fasciculus.CodeAnalysis.Indexing;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.IO;
using Fasciculus.IO.Searching;
using Fasciculus.Net.Navigating;
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

        private readonly SyntaxDebugger syntaxDebugger;
        private readonly ProductionDebugger productionDebugger;
        private readonly ModifierDebugger modifierDebugger;

        public CodeAnalyzerTests()
        {
            syntaxDebugger = new();
            productionDebugger = new(syntaxDebugger);
            modifierDebugger = new();
        }

        [TestMethod]
        public void Test()
        {
            CodeAnalyzerResult result = CodeAnalyzer.Create()
                .WithProjects(GetProjects())
                .WithNodeDebugger(productionDebugger)
                .WithModifierDebugger(modifierDebugger)
                .Build().Analyze();

            LogUnhandledSyntax();
            LogUnhandledModifiers();

            IEnumerable<Symbol> symbols = result.Index.Symbols;
            int namespaceCount = symbols.Where(x => x.Kind == SymbolKind.Namespace).Count();
            int classCount = symbols.Where(x => x.Kind == SymbolKind.Class).Count();
            int enumCount = symbols.Where(x => x.Kind == SymbolKind.Enum).Count();

            Assert.AreEqual(2, result.Packages.Count);
            Assert.AreEqual(37, namespaceCount);
            Assert.AreEqual(114, classCount);
            Assert.AreEqual(2, enumCount);

            //LogProductions();
            //LogUnhandledCommentElements();
            //LogComments(result.Indices);
        }

        public void LogProductions()
        {
            List<ProductionDebuggerEntry> productions = productionDebugger[SyntaxKind.ObjectCreationExpression];

            if (productions.Count == 0)
            {
                return;
            }

            Log("--- productions ---");

            bool hasStructuredTrivia = productions.Any(p => p.HasStructuredTrivia);

            Log($"HasStructuredTrivia: {hasStructuredTrivia}");
            productions.Apply(p => Log(p.ToString()));
        }

        public void LogUnhandledSyntax()
        {
            Dictionary<SyntaxKind, SortedSet<SyntaxKind>> unhandled = syntaxDebugger.GetUnhandled();

            if (unhandled.Count > 0)
            {
                Log("--- unhandled syntax kinds ---");

                foreach (var kvp in unhandled)
                {
                    Log($"{kvp.Key}");

                    string[] kinds = [.. kvp.Value.Select(k => k.ToString()).OrderBy(x => x)];

                    foreach (var kind in kinds)
                    {
                        Log($"- {kind}");
                    }
                }
            }
        }

        public void LogComments(SymbolIndex index)
        {
            UriPath[] paths =
            [
                new("Fasciculus.Core", "Fasciculus.Net.Navigating", "UriPath"),
                new("Fasciculus.Core", "Fasciculus.Algorithms"),
                new("Fasciculus.Core", "Fasciculus.Collections", "ObservableNotifyingEnumerable-1")
            ];

            foreach (UriPath path in paths)
            {
                Symbol symbol = index[path];

                Log($"~~~~ {symbol.Name} ~~~~");
                Log(symbol.Comment.Summary);
            }

            Log("~~~~~~~~~~~~~~~~~~~~~~~~");
        }

        public void LogUnhandledModifiers()
        {
            SortedSet<string> unhandled = modifierDebugger.GetUnhandled();

            if (unhandled.Count > 0)
            {
                Log("--- unhandled modifiers ---");
                Log(string.Join(Environment.NewLine, unhandled.Select(u => "- " + u)));
            }
        }

        public void LogUnhandledCommentElements()
        {
            SortedSet<string> unhandled = UnhandledCommentElements.Instance.Unhandled();

            if (unhandled.Count > 0)
            {
                Log("--- unhandled comment elements ---");
                Log(string.Join(Environment.NewLine, unhandled.Select(u => "- " + u)));
            }
        }

        private static IEnumerable<CodeAnalyzerProject> GetProjects()
        {
            foreach (var projectName in projectNames)
            {
                DirectoryInfo directory = DirectorySearch.Search(projectName, searchPath).First();

                CodeAnalyzerProject project = new()
                {
                    ProjectFile = directory.File(projectName + ".csproj"),
                    RepositoryDirectory = new("tree", "main", "source", projectName)
                };

                yield return project;
            }
        }
    }
}
