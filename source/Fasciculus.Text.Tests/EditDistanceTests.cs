using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Text.Tests
{
    [TestClass]
    public class EditDistanceTests
    {
        [TestMethod]
        public void Test()
        {
            Assert.AreEqual(2, string.Empty.EditDistance("ab"));
            Assert.AreEqual(3, "GEEXSFRGEEKKS".EditDistance("GEEKSFORGEEKS"));
        }
    }
}
