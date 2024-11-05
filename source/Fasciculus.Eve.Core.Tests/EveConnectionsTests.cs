using Fasciculus.Eve.Models;
using Fasciculus.Mathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Eve.Core.Tests
{
    [TestClass]
    public class EveConnectionsTests : EveCoreTests
    {
        [TestMethod]
        public void TestConnections()
        {
            EveConnections connections = EveConnections.Create(universe);

            EveSolarSystem urlen = universe.SolarSystems["Urlen"];
            EveSolarSystem kusomonmon = universe.SolarSystems["Kusomonmon"];
            SparseBoolMatrix matrix = connections.GetSolarSystemMatrix(EveSecurity.High);
            bool connected = matrix[urlen.Index][kusomonmon.Index];

            Assert.IsTrue(connected);
        }
    }
}
