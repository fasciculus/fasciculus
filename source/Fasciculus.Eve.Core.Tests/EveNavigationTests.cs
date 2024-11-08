using Fasciculus.Eve.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Fasciculus.Eve.Core.Tests
{
    [TestClass]
    public class EveNavigationTests : EveCoreTests
    {
        [TestMethod]
        public void TestGetMaxDistance()
        {
            Assert.AreEqual(84, navigation.GetMaxDistance(EveSecurity.High));
            Assert.AreEqual(84, navigation.GetMaxDistance(EveSecurity.LowAndHigh));
            Assert.AreEqual(84, navigation.GetMaxDistance(EveSecurity.All));
        }

        [TestMethod]
        public void TestGetDistance()
        {
            EveSolarSystems solarSystems = universe.SolarSystems;
            EveSolarSystem origin = solarSystems["Jita"];

            Assert.AreEqual(3, navigation.GetDistance(origin, solarSystems["Kusomonmon"], EveSecurity.High));
        }

        [TestMethod]
        public void TestAtRange()
        {
            EveSolarSystem origin = universe.SolarSystems["Jita"];

            TestAtRange(origin, 1, "Perimeter");
            TestAtRange(origin, 2, "Urlen");
            TestAtRange(origin, 3, "Kusomonmon");
            TestAtRange(origin, 4, "Suroken");
            TestAtRange(origin, 5, "Haatomo");
        }

        private static void TestAtRange(EveSolarSystem origin, int distance, string expected)
        {
            string[] actual = navigation.AtRange(origin, distance, EveSecurity.High).Select(ss => ss.Name).ToArray();

            Assert.IsTrue(actual.Contains(expected), $"{origin.Name}({distance}) doesn't contain {expected}");
        }
    }
}
