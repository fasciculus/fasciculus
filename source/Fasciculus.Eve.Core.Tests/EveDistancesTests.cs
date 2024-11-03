using Fasciculus.Eve.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Eve.Core.Tests
{
    [TestClass]
    public class EveDistancesTests : EveCoreTests
    {
        [TestMethod]
        public void TestCreateDistances()
        {
            EveDistances distances = EveDistances.Create(universe, 0.5);

            Assert.AreEqual(41, distances.GetMaxDistance()); // highsec
            //Assert.AreEqual(69, distances.GetMaxDistance()); // lowsec
            // Assert.AreEqual(172, distances.GetMaxDistance()); // nullsec
        }
    }
}
