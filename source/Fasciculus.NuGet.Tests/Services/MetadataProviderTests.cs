using Fasciculus.NuGet.Logging;
using Fasciculus.NuGet.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace Fasciculus.NuGet.Tests.Services
{
    [TestClass]
    public class MetadataProviderTests
    {
        [TestMethod]
        public void Test()
        {
            using SourceCacheContext cache = new();

            NuGetResources resources = new();
            ConsoleLogger logger = new();
            MetadataProvider metadataProvider = new(resources, cache, logger);

            PackageIdentity package = new("NuGet.Protocol", NuGetVersion.Parse("6.12.1"));
            IPackageSearchMetadata metadata = metadataProvider.GetMetadata(package);

            string expectedLicense = "Apache-2.0";
            string actualLicense = metadata.LicenseMetadata.LicenseExpression.ToString() ?? string.Empty;

            Assert.AreEqual(expectedLicense, actualLicense);
        }
    }
}
