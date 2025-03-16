using Fasciculus.IO;
using Fasciculus.NuGet.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class SettingsSearchTests
    {
        [TestMethod]
        public void Test()
        {
            DirectoryInfo? expected = FileSearch.Search("NuGet.config", SearchPath.WorkingDirectoryAndParents()).FirstOrDefault()?.Directory;
            DirectoryInfo? actual = SettingsSearch.Search();

            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);

            Assert.AreEqual(expected.FullName, actual.FullName);
        }
    }
}
