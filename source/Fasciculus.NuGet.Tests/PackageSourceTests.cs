using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class PackageSourceTests
    {
        [TestMethod]
        public void TestLocal()
        {
            PackageSource? packageSource = SettingsLoader.Load().GetLocalPackageSource();

            Assert.IsNotNull(packageSource);

            SourceRepository repository = packageSource.GetRepository();

            Assert.AreEqual(2, repository.PackageSource.ProtocolVersion);
        }

        [TestMethod]
        public void TestRemote()
        {
            PackageSources packageSources = SettingsLoader.Load().GetRemotePackageSources();
            SourceRepositories repositories = packageSources.GetRepositories();

            Assert.AreEqual(1, repositories.Count);
        }
    }
}
