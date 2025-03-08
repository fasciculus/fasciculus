using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Frameworks;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    public class FrameworkTests
    {
        [TestMethod]
        public void Test()
        {
            NuGetFramework net20 = NuGetFramework.Parse("netstandard2.0");
            NuGetFramework net90 = NuGetFramework.Parse("net9.0");
            NuGetFramework net90w = NuGetFramework.Parse("net9.0-windows10.0.19041.0");

            NuGetFramework[] frameworks = [net20, net90, net90w];

            foreach (var framework in frameworks)
            {
                string shortFolderName = framework.GetShortFolderName();
                string versionString = FrameworkNameHelpers.GetVersionString(framework.Version);

                Assert.IsFalse(string.IsNullOrEmpty(shortFolderName));
                Assert.IsFalse(string.IsNullOrEmpty(versionString));
            }
        }
    }
}
