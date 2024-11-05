using Fasciculus.Eve.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Fasciculus.Eve.Core.Tests
{
    [TestClass]
    public class EveSolarSystemTests : EveCoreTests
    {
        [TestMethod]
        public void TestNeighbours()
        {
            EveSolarSystem urlen = universe.SolarSystems["Urlen"];

            EveSolarSystem[] hsn = urlen.GetNeighbours(EveSecurity.High).ToArray();
            EveSolarSystem[] lsn = urlen.GetNeighbours(EveSecurity.LowAndHigh).ToArray();
            EveSolarSystem[] nsn = urlen.GetNeighbours(EveSecurity.All).ToArray();

            Assert.IsTrue(hsn.Select(ss => ss.Name).Contains("Kusomonmon"));
            Assert.IsTrue(lsn.Select(ss => ss.Name).Contains("Kusomonmon"));
            Assert.IsTrue(nsn.Select(ss => ss.Name).Contains("Kusomonmon"));
        }
    }
}
