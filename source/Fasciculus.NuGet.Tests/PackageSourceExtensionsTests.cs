using Fasciculus.NuGet.Configuration;
using Fasciculus.NuGet.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
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
            PackageSource? packageSource = settings.GetLocalPackageSource();

            Assert.IsNotNull(packageSource);

            SourceRepository sourceRepository = packageSource.GetRepository();

            string expected = packageSource.Source;
            string actual = sourceRepository.PackageSource.Source;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestRemote()
        {
            ISettings settings = SettingsLoader.Load();
            PackageSources packageSources = settings.GetRemotePackageSources();
            SourceRepositories sourceRepositories = packageSources.GetRepositories();

            string[] expected = [.. packageSources.Select(x => x.Source)];
            string[] actual = [.. sourceRepositories.Select(x => x.PackageSource.Source)];

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
