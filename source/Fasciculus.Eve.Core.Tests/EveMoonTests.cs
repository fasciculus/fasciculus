using Fasciculus.Eve.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Eve.Core.Tests
{
    [TestClass]
    public class EveMoonTests : EveCoreTests
    {
        [TestMethod]
        public void TestName()
        {
            EveSolarSystem jita = universe.SolarSystems["Jita"];
            EvePlanet planet = jita.Planets[EveCelestialIndex.Create(4)];
            EveMoon moon = planet.Moons[EveCelestialIndex.Create(4)];

            Assert.AreEqual("Jita IV - Moon 4", moon.Name);
        }
    }
}
