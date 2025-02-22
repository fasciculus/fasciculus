using Fasciculus.IO.Searching;
using Microsoft.Build.Construction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Build.Tests
{
    [TestClass]
    public class DirectoryPackagesTests
    {
        [TestMethod]
        public void Test()
        {
            FileInfo file = FileSearch.Search("Directory.Packages.props", SearchPath.WorkingDirectoryAndParents()).First();
            ProjectRootElement projectRootElement = ProjectRootElement.Open(file.FullName);
            ICollection<ProjectItemElement> items = projectRootElement.Items;

            Assert.IsTrue(items.Any(item => item.ItemType == "PackageVersion"));
        }
    }
}
