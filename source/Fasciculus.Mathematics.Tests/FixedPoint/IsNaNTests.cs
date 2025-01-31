using Fasciculus.Mathematics.FixedPoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Mathematics.Tests.FixedPoint
{
    [TestClass]
    public class IsNaNTests
    {
        [TestMethod]
        public void TestFP16Q8()
        {
            for (ushort value = 0; value < 0x4001; ++value)
            {
                ushort pos = value;
                ushort neg = (ushort)(value | 0x8000);

                Assert.IsFalse(FP16Q8.IsNaN(pos), "0x{0:X}", pos);
                Assert.IsFalse(FP16Q8.IsNaN(neg), "0x{0:X}", neg);
            }

            Assert.IsFalse(FP16Q8.IsNaN(FP16Q8.PosInf), "PositiveInfinity");
            Assert.IsFalse(FP16Q8.IsNaN(FP16Q8.NegInf), "NegativeInfinity");

            for (ushort value = 1; value < 0x4000; ++value)
            {
                ushort pos = (ushort)(value | 0x4000);
                ushort neg = (ushort)(value | 0xC000);

                Assert.IsTrue(FP16Q8.IsNaN(pos), "0x{0:X}", pos);
                Assert.IsTrue(FP16Q8.IsNaN(neg), "0x{0:X}", neg);
            }
        }
    }
}
