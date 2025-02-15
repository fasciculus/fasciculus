using Fasciculus.NetStandard.Testee;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.NetStandard.Tests
{
    [TestClass]
    public class AttributesTests
    {
        [TestMethod]
        public void TestDisallowNull()
        {
            string? expected = "bar";
            string actual = NetStandardTestee.Foo1(expected);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestNotNullWhen()
        {
            string? input;
            string? output;

            input = null;
            bool result1 = NetStandardTestee.Foo2(input, out output);

            Assert.IsFalse(result1);
            Assert.IsNull(output);

            input = "bar";

            if (NetStandardTestee.Foo2(input, out output))
            {
                string actual = output;

                Assert.IsNotNull(output);
                Assert.AreEqual(input, actual);
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}
