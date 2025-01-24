using Fasciculus.NuGet.Logging;
using Fasciculus.NuGet.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Collections.Generic;

namespace Fasciculus.NuGet.Tests.Services
{
    [TestClass]
    public class NuGetVersionsTests
    {
        [TestMethod]
        public void Test()
        {
            using SourceCacheContext cache = new();

            NuGetResources resources = new();
            ConsoleLogger logger = new();
            VersionsProvider versionsProvider = new(resources, cache, logger);

            SortedSet<NuGetVersion> versions = versionsProvider.GetVersions("NuGet.Protocol", false);

            Assert.AreEqual(83, versions.Count);
        }
    }
}
