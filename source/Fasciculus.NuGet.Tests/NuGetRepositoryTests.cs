using Fasciculus.NuGet.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class NuGetRepositoryTests
    {
        [TestMethod]
        public async Task TestLocal()
        {
            NuGetRepository repository = await NuGetRepository.Local;

            Assert.IsTrue(repository.Repository.PackageSource.IsLocal);
        }

        [TestMethod]
        public async Task TestDefault()
        {
            NuGetRepositories repositories = await NuGetRepositories.Remotes;

            foreach (NuGetRepository repository in repositories)
            {
                Assert.IsTrue(repository.Repository.PackageSource.IsEnabled);
            }
        }
    }
}
