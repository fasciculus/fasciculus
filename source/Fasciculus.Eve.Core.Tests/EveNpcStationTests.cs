using Fasciculus.Eve.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Core.Tests
{
    [TestClass]
    public class EveNpcStationTests : EveCoreTests
    {
        [TestMethod]
        public void TestNames()
        {
            EveSolarSystem jita = universe.SolarSystems["Jita"];
            EvePlanet planet = jita.Planets[EveCelestialIndex.Create(4)];
            EveMoon moon = planet.Moons[EveCelestialIndex.Create(4)];
            IReadOnlyCollection<EveNpcStation> npcStations = moon.NpcStations;

            Assert.AreEqual(2, npcStations.Count);

            EveNpcStation npcStation = npcStations.Last();

            Assert.AreEqual("Jita IV - Moon 4 - Caldari Navy Assembly Plant", npcStation.Name);
        }
    }
}
