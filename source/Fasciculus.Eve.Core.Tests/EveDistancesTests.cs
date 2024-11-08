using Fasciculus.Eve.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Fasciculus.Eve.Core.Tests
{
    [TestClass]
    public class EveDistancesTests : EveCoreTests
    {
        [TestMethod]
        public void TestCreateDistances()
        {
            // Assert.AreEqual(84, navigation.GetMaxDistance(EveSecurity.High)); // highsec
            //Assert.AreEqual(69, distances.GetMaxDistance()); // lowsec
            // Assert.AreEqual(172, distances.GetMaxDistance()); // nullsec
        }

        [TestMethod]
        public void TestHighSecDistance()
        {
            EveSolarSystems solarSystems = universe.SolarSystems;
            EveSolarSystem origin = solarSystems["Jita"];

            // Assert.AreEqual(3, distances.GetDistance(origin, solarSystems["Kusomonmon"]));
        }

        [TestMethod]
        public void TestHighSecAtRange()
        {
            EveSolarSystem origin = universe.SolarSystems["Jita"];

            //TestAtRange(distances, origin, 1, "Perimeter");
            //TestAtRange(distances, origin, 2, "Urlen");
            //TestAtRange(distances, origin, 3, "Kusomonmon");
            //TestAtRange(distances, origin, 4, "Suroken");
            //TestAtRange(distances, origin, 5, "Haatomo");
        }

        private static void TestAtRange(EveDistances distances, EveSolarSystem origin, int distance, string expected)
        {
            string[] actual = distances.AtRange(origin, distance).Select(ss => ss.Name).ToArray();

            Assert.IsTrue(actual.Contains(expected), $"{origin.Name}({distance}) doesn't contain {expected}");
        }
    }
}
