using Fasciculus.NuGet.Configuration;
using Fasciculus.NuGet.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Configuration;
using System.Linq;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class PackageSourceExtensionsTests
    {
        [TestMethod]
        public void TestLocal()
        {
            ISettings settings = SettingsLoader.Load();
            NuGetSource packageSource = settings.GetLocalPackageSource();
            NuGetRepository sourceRepository = packageSource.GetRepository();

            string expected = packageSource.Source.Source;
            string actual = sourceRepository.Repository.PackageSource.Source;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestRemote()
        {
            ISettings settings = SettingsLoader.Load();
            NuGetSources packageSources = settings.GetRemotePackageSources();
            NuGetRepositories sourceRepositories = packageSources.GetRepositories();

            string[] expected = [.. packageSources.Select(x => x.Source.Source)];
            string[] actual = [.. sourceRepositories.Select(x => x.Repository.PackageSource.Source)];

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
