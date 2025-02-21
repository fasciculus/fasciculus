using Fasciculus.IO.Searching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Configuration;
using System.IO;
using System.Linq;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class SettingsTests
    {
        [TestMethod]
        public void Test()
        {
            FileInfo configFile = FileSearch.Search("NuGet.config", SearchPath.WorkingDirectoryAndParents()).First();
            ISettings settings = SettingsLoader.Load();
            SettingSection? section = settings.GetSection("config");
            AddItem? item = section?.GetFirstItemWithAttribute<AddItem>("key", "repositoryPath");

            Assert.IsNotNull(item);
            Assert.IsNotNull(item.ConfigPath);

            string expected = configFile.FullName.ToLower();
            string actual = item.ConfigPath.ToLower();

            Assert.AreEqual(expected, actual);
        }
    }
}
