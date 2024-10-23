using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Eve.Core.Tests
{
    [TestClass]
    public class EveUniverseTests : EveCoreTests
    {
        [TestMethod]
        public void TestRegionCount()
        {
            Assert.AreEqual(69, universe.Regions.Count);
        }
    }
}
