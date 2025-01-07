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
        public void TestOldParser()
        {
            using MSBuildWorkspace workspace = LoadWorkspace(["Fasciculus.Core", "Fasciculus.Extensions"]);
            PackageCollection packages = ProjectParser.Parse(workspace.CurrentSolution.Projects, false);

            Assert.AreEqual(2, packages.Count());

            foreach (PackageInfo package in packages)
            {
                Assert.AreEqual(3, package.Frameworks.Count());
                Assert.IsTrue(package.Namespaces.Count() > 1);

                foreach (NamespaceInfo @namespace in package.Namespaces)
                {
                    Assert.AreEqual(3, @namespace.Frameworks.Count());
                }
            }

            PackageInfo corePackage = packages.First(p => p.Name == "Fasciculus.Core");
            NamespaceInfo collectionsNamespace = corePackage.Namespaces.First(n => n.Name == "Fasciculus.Collections");
            ClassInfo bitsetClass = collectionsNamespace.Classes.First(c => c.Name == "BitSet");
            ClassInfo disposableStackClass = collectionsNamespace.Classes.First(c => c.Name == "DisposableStack<T>");

            Assert.AreEqual(7, collectionsNamespace.Classes.Count());
            Assert.AreEqual(3, bitsetClass.Frameworks.Count());
            Assert.AreEqual(3, disposableStackClass.Frameworks.Count());
        }

        [TestMethod]
        public void TestParser()
        {
            using MSBuildWorkspace workspace = LoadWorkspace(["Fasciculus.Core", "Fasciculus.Extensions"]);
            Project project = workspace.CurrentSolution.Projects.First(p => p.HasDocuments);
            ParsedProject[] parsedProjects = ProjectParser2.Parse(workspace.CurrentSolution.Projects, false);
            PackageList packages = PackageCompiler.Compile(parsedProjects);
            PackageSymbol merged = packages.Merge("Combined");

            ReportNodeKindReporter();
        }

        private void ReportNodeKindReporter()
        {
            StringBuilder stringBuilder = new();
            using StringWriter writer = new(stringBuilder);

            if (NodeKindReporter.Instance.Report(writer))
            {
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
