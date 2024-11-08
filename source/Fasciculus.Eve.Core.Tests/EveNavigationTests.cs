using Fasciculus.Eve.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
