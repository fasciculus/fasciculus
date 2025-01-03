using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Frameworks;
using System;

namespace Fasciculus.CodeAnalysis.Tests
{
    [TestClass]
    public class FrameworkTests : TestsBase
    {
        [TestMethod]
        public void Test()
        {
            NuGetFramework netstandard2 = NuGetFramework.Parse("netstandard2.0");
            Version netstandard2Version = new("2.0.0.0");

            NuGetFramework net9 = NuGetFramework.Parse("net9.0");
            Version net9Version = new("9.0.0.0");

            NuGetFramework net9win = NuGetFramework.Parse("net9.0-windows10.0.19041.0");
            Version net9winVersion = new("9.0.0.0");
            Version net9winPlatformVersion = new("10.0.19041.0");

            Assert.AreEqual(".NETStandard", netstandard2.Framework);
            Assert.AreEqual(netstandard2Version, netstandard2.Version);

            Assert.AreEqual(".NETCoreApp", net9.Framework);
            Assert.AreEqual(net9Version, net9.Version);

            Assert.AreEqual(".NETCoreApp", net9win.Framework);
            Assert.AreEqual(net9winVersion, net9win.Version);
            Assert.AreEqual("windows", net9win.Platform);
            Assert.AreEqual(net9winPlatformVersion, net9win.PlatformVersion);
        }
    }
}
