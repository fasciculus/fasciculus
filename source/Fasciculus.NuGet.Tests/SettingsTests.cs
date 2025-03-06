using Fasciculus.IO;
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
        public void TestConfigPath()
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

        [TestMethod]
        public void TestLocalPackageSource()
        {
            PackageSource? packageSource = SettingsLoader.Load().GetLocalPackageSource();

            Assert.IsNotNull(packageSource);

            FileInfo configFile = FileSearch.Search("NuGet.config", SearchPath.WorkingDirectoryAndParents()).First();
            DirectoryInfo directory = configFile.Directory!.Combine(".repository", "packages", "installed");

            string expected = directory.FullName.ToLower();
            string actual = packageSource.Source.ToLower();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestRemotePackageSources()
        {
            PackageSources packageSources = SettingsLoader.Load().GetRemotePackageSources();

            Assert.AreEqual(1, packageSources.Count);
        }
    }
}
