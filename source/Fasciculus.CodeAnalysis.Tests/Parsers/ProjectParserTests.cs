using Fasciculus.CodeAnalysis.Models;
using Fasciculus.CodeAnalysis.Parsers;
using Fasciculus.CodeAnalysis.Workspaces;
using Fasciculus.IO.Searching;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Tests.Parsers
{
    [TestClass]
    public class ProjectParserTests
    {
        [TestMethod]
        public void TestParse()
        {
            using MSBuildWorkspace workspace = WorkspaceFactory.CreateWorkspace();

            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents;
            string[] projectNames = ["Fasciculus.Core", "Fasciculus.Extensions"];

            projectNames
                .Select(n => Tuple.Create(n, DirectorySearch.Search(n, searchPath).First()))
                .Select(t => t.Item2.File(t.Item1 + ".csproj"))
                .Apply(f => { workspace.AddProjectFile(f); });

            Packages packages = ProjectParser.Parse(workspace.CurrentSolution.Projects, false);

            Assert.AreEqual(2, packages.Count());

            foreach (PackageInfo package in packages)
            {
                Assert.AreEqual(3, package.Frameworks.Count());

                foreach (NamespaceInfo @namespace in package.Namespaces)
                {
                    Assert.AreEqual(3, @namespace.Frameworks.Count());
                }
            }
        }
    }
}
