using Fasciculus.Collections;
using Fasciculus.NuGet.Frameworks;
using Fasciculus.NuGet.Logging;
using Fasciculus.NuGet.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fasciculus.NuGet.Tests.Services
{
    [TestClass]
    public class DependencyProviderTests
    {
        [TestMethod]
        public void Test()
        {
            using SourceCacheContext cache = new();
            NuGetResources resources = new();
            ConsoleLogger logger = new(LogLevel.Warning);
            VersionsProvider versionsProvider = new(resources, cache, logger);
            MetadataProvider metadataProvider = new(resources, cache, logger);
            IgnoredPackages ignoredPackages = new();
            DependencyProvider dependencyProvider = new(metadataProvider, ignoredPackages, logger);
            PackageIdentity package = new("NuGet.Protocol", NuGetVersion.Parse("6.12.1"));
            NuGetFramework targetFramework = MoreFrameworks.Net90;

            IPackageSearchMetadata[] dependencies = dependencyProvider.GetDependencies([package], targetFramework);
            SortedSet<string> dependencyIds = new(dependencies.Select(x => x.Identity.Id));

            dependencyIds.Apply(x => { Debug.WriteLine(x); });

            SortedSet<string> licenses = new(dependencies
                .Select(x => x.LicenseMetadata)
                .NotNull()
                .Select(x => x.License));

            Assert.AreEqual(9, dependencies.Length);
            Assert.AreEqual(2, licenses.Count);
        }
    }
}
