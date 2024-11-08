using Fasciculus.Eve.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Core.Tests
{
    [TestClass]
    public class EveConstellationTests : EveCoreTests
    {
        [TestMethod]
        public void TestNeighbours()
        {
            EveSolarSystem jita = universe.SolarSystems["Jita"];
            EveConstellation constellation = jita.Constellation;

            IEnumerable<EveConstellation> neighbours = constellation.GetNeighbours(EveSecurity.All);
            string[] names = neighbours.Select(x => x.Name).ToArray();

            Assert.IsTrue(names.Contains("Ihilakken"));
        }
    }
}
