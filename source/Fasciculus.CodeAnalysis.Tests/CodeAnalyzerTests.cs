using Fasciculus.CodeAnalysis.Configuration;
using Fasciculus.CodeAnalysis.Debugging;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.IO;
using Fasciculus.IO.Searching;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Tests
{
    public class CodeAnalyzerTests : TestsBase
    {
        protected void Test(CodeAnalyzerTestContext context)
        {
            ICodeAnalyzerResult result = CodeAnalyzer.Create()
                .WithProjects(context.Projects)
                .WithDebuggers(context.Debuggers)
                .Build().Analyze();

            IEnumerable<ISymbol> symbols = result.Index.Symbols;
            INamespaceSymbol[] namespaces = [.. symbols.Where(x => x.Kind == SymbolKind.Namespace).Cast<INamespaceSymbol>()];
            IEnumSymbol[] enums = [.. symbols.Where(x => x.Kind == SymbolKind.Enum).Cast<IEnumSymbol>()];
            IInterfaceSymbol[] interfaces = [.. symbols.Where(x => x.Kind == SymbolKind.Interface).Cast<IInterfaceSymbol>()];
            IClassSymbol[] classes = [.. symbols.Where(x => x.Kind == SymbolKind.Class).Cast<IClassSymbol>()];

            IFieldSymbol[] fields = [.. symbols.Where(x => x.Kind == SymbolKind.Field).Cast<IFieldSymbol>()];
            IMemberSymbol[] members = [.. symbols.Where(x => x.Kind == SymbolKind.Member).Cast<IMemberSymbol>()];
            IEventSymbol[] events = [.. symbols.Where(x => x.Kind == SymbolKind.Event).Cast<IEventSymbol>()];
            IPropertySymbol[] properties = [.. symbols.Where(x => x.Kind == SymbolKind.Property).Cast<IPropertySymbol>()];

            IConstructorSymbol[] constructors = [.. symbols.Where(x => x.Kind == SymbolKind.Constructor).Cast<IConstructorSymbol>()];
            IMethodSymbol[] methods = [.. symbols.Where(x => x.Kind == SymbolKind.Method).Cast<IMethodSymbol>()];

            string[] summaries = [.. symbols.Select(x => x.Comment.Summary).Where(x => !string.IsNullOrEmpty(x))];

            LogProductions(context);
            LogUnhandledSyntax(context);
            LogUnhandledModifiers(context);
            LogUnhandledAccessors(context);
            LogUnhandledComments(context);

            Assert.AreEqual(context.Packages, result.Packages.Length);

            Assert.AreEqual(context.Namespaces, namespaces.Length, "Namespaces");
            Assert.AreEqual(context.Enums, enums.Length, "Enums");
            Assert.AreEqual(context.Interfaces, interfaces.Length, "Interfaces");
            Assert.AreEqual(context.Classes, classes.Length, "Classes");

            Assert.AreEqual(context.Fields, fields.Length, "Fields");
            Assert.AreEqual(context.Members, members.Length, "Members");
            Assert.AreEqual(context.Events, events.Length, "Events");
            Assert.AreEqual(context.Properties, properties.Length, "Properties");
            Assert.IsTrue(properties.All(p => p.Accessors.Any()));

            Assert.AreEqual(context.Constructors, constructors.Length, "Constructors");
            Assert.AreEqual(context.Methods, methods.Length, "Methods");

            Assert.AreEqual(context.Summaries, summaries.Length, "Summaries");

            Assert.AreEqual(0, context.SyntaxDebugger.GetUnhandled().Count, "SyntaxDebugger");
            Assert.AreEqual(0, context.ModifierDebugger.GetUnhandled().Count, "ModifierDebugger");
            Assert.AreEqual(0, context.AccessorDebugger.GetUnhandled().Count, "AccessorDebugger");
            Assert.AreEqual(0, context.CommentDebugger.GetUnhandled().Count, "CommentDebugger");
        }

        private void LogProductions(CodeAnalyzerTestContext context)
        {
            List<ProductionDebuggerEntry> productions = context.ProductionDebugger[context.ProductionKind];

            if (productions.Count == 0)
            {
                return;
            }

            Log("--- productions ---");

            bool hasStructuredTrivia = productions.Any(p => p.HasStructuredTrivia);

            Log($"HasStructuredTrivia: {hasStructuredTrivia}");
            productions.Apply(p => Log(p.ToString()));
        }

        private void LogUnhandledSyntax(CodeAnalyzerTestContext context)
        {
            Dictionary<SyntaxKind, SortedSet<SyntaxKind>> unhandled = context.SyntaxDebugger.GetUnhandled();

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

        private void LogUnhandledModifiers(CodeAnalyzerTestContext context)
        {
            SortedSet<string> unhandled = context.ModifierDebugger.GetUnhandled();

            if (unhandled.Count > 0)
            {
                Log("--- unhandled modifiers ---");
                Log(string.Join(Environment.NewLine, unhandled.Select(u => "- " + u)));
            }
        }

        private void LogUnhandledAccessors(CodeAnalyzerTestContext context)
        {
            SortedSet<SyntaxKind> unhandled = context.AccessorDebugger.GetUnhandled();

            if (unhandled.Count > 0)
            {
                Log("--- unhandled accessors ---");
                Log(string.Join(Environment.NewLine, unhandled.Select(u => "- " + u)));
            }
        }

        private void LogUnhandledComments(CodeAnalyzerTestContext context)
        {
            SortedSet<string> unhandled = context.CommentDebugger.GetUnhandled();

            if (unhandled.Count > 0)
            {
                Log("--- unhandled comment elements ---");
                Log(string.Join(Environment.NewLine, unhandled.Select(u => "- " + u)));
            }
        }

        protected static CodeAnalyzerProject GetProject(string name)
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
