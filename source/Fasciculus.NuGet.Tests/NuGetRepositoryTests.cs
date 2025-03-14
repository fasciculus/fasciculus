using Fasciculus.NuGet.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class NuGetRepositoryTests
    {
        [TestMethod]
        public async Task TestGlobal()
        {
            NuGetRepository repository = await NuGetRepository.Global;

            Assert.IsTrue(repository.Repository.PackageSource.IsLocal);
        }

        [TestMethod]
        public async Task TestDefault()
        {
            NuGetRepositories repositories = await NuGetRepositories.Default;

            foreach (NuGetRepository repository in repositories)
            {
                Assert.IsTrue(repository.Repository.PackageSource.IsEnabled);
            }
        }
    }
}
