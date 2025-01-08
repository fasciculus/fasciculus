using Fasciculus.CodeAnalysis.Compilers;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.CodeAnalysis.Parsers;
using Fasciculus.CodeAnalysis.Support;
using Fasciculus.CodeAnalysis.Workspaces;
using Fasciculus.Collections;
using Fasciculus.IO;
using Fasciculus.IO.Searching;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fasciculus.CodeAnalysis.Tests.Parsers
{
    [TestClass]
    public class ProjectParserTests : TestsBase
    {
        [TestMethod]
        public void TestParser()
        {
            using MSBuildWorkspace workspace = LoadWorkspace(["Fasciculus.Core", "Fasciculus.Extensions"]);
            ParsedProject[] parsedProjects = ProjectParser2.Parse(workspace.CurrentSolution.Projects, false);
            PackageList packages = PackageCompiler.Compile(parsedProjects);
            PackageSymbol merged = packages.Combine("Combined");

            ReportComment(merged);
            ReportNodeKindReporter();
        }

        private void ReportComment(PackageSymbol merged)
        {
            NamespaceSymbol ns = merged.Namespaces.First(n => n.Name.Name == "Fasciculus.Collections");
            ClassSymbol c = ns.Classes.First(c => c.Name.Name == "ObservableNotifyingEnumerable<T>");

            Log("--- Comment ---");
            Log(c.Comment);
            Log("--- Comment ---");
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

        private static MSBuildWorkspace LoadWorkspace(IEnumerable<string> projectNames)
        {
            MSBuildWorkspace workspace = WorkspaceFactory.CreateWorkspace();
            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents;

            projectNames
                .Select(n => Tuple.Create(DirectorySearch.Search(n, searchPath).First(), n))
                .Select(t => t.Item1.File(t.Item2 + ".csproj"))
                .Apply(f => { workspace.AddProjectFile(f); });

            return workspace;
        }
    }
}
