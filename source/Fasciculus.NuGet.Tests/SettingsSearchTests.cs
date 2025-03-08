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
            FileInfo expected = FileSearch.Search("NuGet.config", SearchPath.WorkingDirectoryAndParents()).First();
            FileInfo? actual = SettingsSearch.Search();

            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.FullName, actual.FullName);
        }
    }
}
