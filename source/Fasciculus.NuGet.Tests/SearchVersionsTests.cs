using Fasciculus.NuGet.Configuration;
using Fasciculus.NuGet.Protocol;
using Fasciculus.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Configuration;
using NuGet.Versioning;
using System.Collections.Generic;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class SearchVersionsTests
    {
        [TestMethod]
        public void Test()
        {
            ISettings settings = SettingsLoader.Load();
            NuGetSources packageSources = settings.GetRemotePackageSources();
            NuGetRepositories repositories = packageSources.GetRepositories();
            NuGetResources resources = repositories.Resources;
            SortedSet<NuGetVersion> versions = Tasks.Wait(NuGetSearch.SearchVersionsAsync(resources, "NuGet.Protocol"));

            Assert.IsTrue(versions.Count > 0);
        }
    }
}
