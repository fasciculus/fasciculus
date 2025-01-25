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
    [TestClass]
    public class CodeAnalyzerTests : TestsBase
    {
        private class TestContext
        {
            public required List<CodeAnalyzerProject> Projects { get; init; }

            public required SyntaxKind ProductionKind { get; init; }

            public required int Packages { get; init; }
            public required int Namespaces { get; init; }
            public required int Enums { get; init; }
            public required int Interfaces { get; init; }
            public required int Classes { get; init; }

            public required int Fields { get; init; }
            public required int Members { get; init; }
            public required int Events { get; init; }
            public required int Properties { get; init; }

            public required int Constructors { get; init; }

            public required int Summaries { get; init; }

            public DefaultSyntaxDebugger SyntaxDebugger { get; }
            public DefaultProductionDebugger ProductionDebugger { get; }
            public DefaultModifierDebugger ModifierDebugger { get; }
            public DefaultAccessorDebugger AccessorDebugger { get; }

            public CodeAnalyzerDebuggers Debuggers { get; }

            public TestContext()
            {
                SyntaxDebugger = new();
                ProductionDebugger = new(SyntaxDebugger);
                ModifierDebugger = new();
                AccessorDebugger = new();

                Debuggers = new()
                {
                    NodeDebugger = ProductionDebugger,
                    ModifierDebugger = ModifierDebugger,
                    AccessorDebugger = AccessorDebugger
                };
            }
        }

        [TestMethod]
        public void TestFasciculusCore()
        {
            TestContext context = new()
            {
                Projects = [GetProject("Fasciculus.Core")],
                ProductionKind = SyntaxKind.None,

                Packages = 1,
                Namespaces = 34,
                Enums = 2,
                Interfaces = 8,
                Classes = 110,

                Fields = 6,
                Members = 6,
                Events = 4,
                Properties = 102,

                Constructors = 62,

                Summaries = 306,
            };

            Test(context);
        }

        [TestMethod]
        public void TestFasciculusExtensions()
        {
            TestContext context = new()
            {
                Projects = [GetProject("Fasciculus.Extensions")],
                ProductionKind = SyntaxKind.None,

                Packages = 1,
                Namespaces = 4,
                Enums = 0,
                Interfaces = 0,
                Classes = 8,

                Fields = 0,
                Members = 0,
                Events = 0,
                Properties = 4,

                Constructors = 4,

                Summaries = 17,
            };

            Test(context);
        }

        private void Test(TestContext context)
        {
            CodeAnalyzerResult result = CodeAnalyzer.Create()
                .WithProjects(context.Projects)
                .WithDebuggers(context.Debuggers)
                .Build().Analyze();

            LogProductions(context);
            LogUnhandledSyntax(context);
            LogUnhandledModifiers(context);
            LogUnhandledAccessors(context);

            IEnumerable<Symbol> symbols = result.Index.Symbols;
            NamespaceSymbol[] namespaces = [.. symbols.Where(x => x.Kind == SymbolKind.Namespace).Cast<NamespaceSymbol>()];
            EnumSymbol[] enums = [.. symbols.Where(x => x.Kind == SymbolKind.Enum).Cast<EnumSymbol>()];
            InterfaceSymbol[] interfaces = [.. symbols.Where(x => x.Kind == SymbolKind.Interface).Cast<InterfaceSymbol>()];
            ClassSymbol[] classes = [.. symbols.Where(x => x.Kind == SymbolKind.Class).Cast<ClassSymbol>()];

            FieldSymbol[] fields = [.. symbols.Where(x => x.Kind == SymbolKind.Field).Cast<FieldSymbol>()];
            MemberSymbol[] members = [.. symbols.Where(x => x.Kind == SymbolKind.Member).Cast<MemberSymbol>()];
            EventSymbol[] events = [.. symbols.Where(x => x.Kind == SymbolKind.Event).Cast<EventSymbol>()];
            PropertySymbol[] properties = [.. symbols.Where(x => x.Kind == SymbolKind.Property).Cast<PropertySymbol>()];

            ConstructorSymbol[] constructors = [.. symbols.Where(x => x.Kind == SymbolKind.Constructor).Cast<ConstructorSymbol>()];

            string[] summaries = [.. symbols.Select(x => x.Comment.Summary).Where(x => !string.IsNullOrEmpty(x))];

            Assert.AreEqual(context.Packages, result.Packages.Count);

            Assert.AreEqual(context.Namespaces, namespaces.Length, "Namespaces");
            Assert.AreEqual(context.Enums, enums.Length, "Enums");
            Assert.AreEqual(context.Interfaces, interfaces.Length, "Interfaces");
            Assert.AreEqual(context.Classes, classes.Length, "Classes");

            Assert.AreEqual(context.Fields, fields.Length, "Fields");
            Assert.AreEqual(context.Members, members.Length, "Members");
            Assert.AreEqual(context.Events, events.Length, "Events");
            Assert.AreEqual(context.Properties, properties.Length, "Properties");

            Assert.AreEqual(context.Constructors, constructors.Length, "Constructors");

            Assert.AreEqual(context.Summaries, summaries.Length, "Summaries");

            Assert.AreEqual(0, context.SyntaxDebugger.GetUnhandled().Count);
            Assert.AreEqual(0, context.ModifierDebugger.GetUnhandled().Count);
            Assert.AreEqual(0, context.AccessorDebugger.GetUnhandled().Count);
        }

        private void LogProductions(TestContext context)
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

        private void LogUnhandledSyntax(TestContext context)
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

        private void LogUnhandledModifiers(TestContext context)
        {
            SortedSet<string> unhandled = context.ModifierDebugger.GetUnhandled();

            if (unhandled.Count > 0)
            {
                Log("--- unhandled modifiers ---");
                Log(string.Join(Environment.NewLine, unhandled.Select(u => "- " + u)));
            }
        }

        private void LogUnhandledAccessors(TestContext context)
        {
            SortedSet<SyntaxKind> unhandled = context.AccessorDebugger.GetUnhandled();

            if (unhandled.Count > 0)
            {
                Log("--- unhandled accessors ---");
                Log(string.Join(Environment.NewLine, unhandled.Select(u => "- " + u)));
            }
        }

        private static CodeAnalyzerProject GetProject(string name)
        {
            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents;
            DirectoryInfo directory = DirectorySearch.Search(name, searchPath).First();

            return new()
            {
                ProjectFile = directory.File(name + ".csproj"),
                RepositoryDirectory = new("tree", "main", "source", name)
            };
        }
    }
}
