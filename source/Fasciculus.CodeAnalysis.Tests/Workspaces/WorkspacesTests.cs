using Fasciculus.CodeAnalysis.Workspaces;
using Fasciculus.IO;
using Fasciculus.IO.Searching;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Tests.Workspaces
{
    [TestClass]
    public class WorkspacesTests
    {
        [TestMethod]
        public void TestAddProjectFiles()
        {
            using MSBuildWorkspace workspace = WorkspaceFactory.CreateWorkspace();

            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents;

            DirectoryInfo projectDirectory1 = DirectorySearch.Search("Fasciculus.Core", searchPath).First();
            FileInfo projectFile1 = projectDirectory1.File("Fasciculus.Core.csproj");

            workspace.AddProjectFile(projectFile1);
            workspace.AddProjectFile(projectFile1);

            DirectoryInfo projectDirectory2 = DirectorySearch.Search("Fasciculus.Extensions", searchPath).First();
            FileInfo projectFile2 = projectDirectory2.File("Fasciculus.Extensions.csproj");

            workspace.AddProjectFile(projectFile2);

            Assert.IsTrue(workspace.HasProjectFile(projectFile1));
            Assert.IsTrue(workspace.HasProjectFile(projectFile2));
        }
    }
}
