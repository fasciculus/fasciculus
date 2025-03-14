using Fasciculus.NuGet.Configuration;
using Fasciculus.NuGet.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Configuration;
using NuGet.Versioning;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class SearchVersionsTests
    {
        [TestMethod]
        public async Task Test()
        {
            ISettings settings = SettingsLoader.Load();
            NuGetSources sources = await NuGetSources.Remotes;
            NuGetRepositories repositories = sources.GetRepositories();
            NuGetResources resources = repositories.Resources;
            SortedSet<NuGetVersion> versions = await NuGetSearch.SearchVersionsAsync(resources, "NuGet.Protocol");

            Assert.IsTrue(versions.Count > 0);
        }
    }
}
