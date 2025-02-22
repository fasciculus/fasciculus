using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class DirectoryPackagesTests
    {
        [TestMethod]
        public void TestSearch()
        {
            FileInfo[] files = DirectoryPackages.Search();

            Assert.AreEqual(1, files.Length);
        }

        [TestMethod]
        public void TestLoad()
        {
            DirectoryPackages packages = DirectoryPackages.Load();

            Assert.IsTrue(packages.Count > 1);
        }
    }
}
