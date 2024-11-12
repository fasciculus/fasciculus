using Fasciculus.Eve.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Fasciculus.Eve.Core.Tests
{
    [TestClass]
    public class EvePlanetTests : EveCoreTests
    {
        [TestMethod]
        public void TestNames()
        {
            EveSolarSystem jita = universe.SolarSystems["Jita"];
            EvePlanet[] planets = jita.Planets.ToArray();

            Assert.AreEqual("Jita I", planets[0].Name);
            Assert.AreEqual("Jita II", planets[1].Name);
            Assert.AreEqual("Jita III", planets[2].Name);
            Assert.AreEqual("Jita IV", planets[3].Name);
            Assert.AreEqual("Jita V", planets[4].Name);
            Assert.AreEqual("Jita VI", planets[5].Name);
            Assert.AreEqual("Jita VII", planets[6].Name);
            Assert.AreEqual("Jita VIII", planets[7].Name);
        }
    }
}
