using Fasciculus.Eve.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Fasciculus.Eve.Core.Tests
{
    [TestClass]
    public class EveTradeOpportunitiesTests : EveCoreTests
    {
        [TestMethod]
        public void Test()
        {
            EveNpcStation origin = universe.SolarSystems["Jita"]
                .Planets[EveCelestialIndex.Create(4)]
                .Moons[EveCelestialIndex.Create(4)]
                .NpcStations.Last();

            EveNpcStation destination = universe.SolarSystems["Dodixie"]
                .Planets[EveCelestialIndex.Create(9)]
                .Moons[EveCelestialIndex.Create(20)]
                .NpcStations.Last();

            TradeOpportunities.Create(data.Types, origin, destination, 5000);
        }
    }
}
