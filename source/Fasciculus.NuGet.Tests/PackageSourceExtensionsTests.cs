using Fasciculus.NuGet.Configuration;
using Fasciculus.NuGet.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class PackageSourceExtensionsTests
    {
        [TestMethod]
        public async Task TestGlobal()
        {
            NuGetSource source = await NuGetSource.Global;
            NuGetRepository repository = source.GetRepository();

            string expected = source.Source.Source;
            string actual = repository.Repository.PackageSource.Source;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task TestDefault()
        {
            NuGetSources sources = await NuGetSources.Default;
            NuGetRepositories sourceRepositories = sources.GetRepositories();

            string[] expected = [.. sources.Select(x => x.Source.Source)];
            string[] actual = [.. sourceRepositories.Select(x => x.Repository.PackageSource.Source)];

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
