using Fasciculus.IO;
using Fasciculus.IO.Searching;
using Fasciculus.NuGet.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Packaging.Core;
using System.IO;
using System.Linq;

namespace Fasciculus.NuGet.Tests.Services
{
    [TestClass]
    public class ProjectPackagesProviderTests
    {
        [TestMethod]
        public void Test()
        {
            string[] packageNames = ["Fasciculus.Core", "Fasciculus.Extensions"];
            FileInfo solutionFile = FileSearch.Search("fasciculus.sln", SearchPath.WorkingDirectoryAndParents()).First();
            DirectoryInfo sourceDirectory = solutionFile.Directory!.Combine("source");
            SearchPath searchPath = new(sourceDirectory.GetDirectories());
            ProjectPackagesProvider provider = new();
            PackageIdentity[] packages = provider.GetPackages(packageNames, searchPath);

            Assert.AreEqual(3, packages.Length);
        }
    }
}
