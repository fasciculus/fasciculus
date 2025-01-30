using Fasciculus.Mathematics.FixedPoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Mathematics.Tests.FixedPoint
{
    [TestClass]
    public class ToDoubleTests
    {
        [TestMethod]
        public void TestFP16Q8()
        {
            double minValue = FP16Q8.ToDouble(FP16Q8.MinValue);
            double maxValue = FP16Q8.ToDouble(FP16Q8.MaxValue);
            double epsilon = FP16Q8.ToDouble(FP16Q8.Epsilon);

            Assert.AreEqual(-32.0, minValue);
            Assert.AreEqual(32.0, maxValue);
            Assert.AreEqual(1.0 / 256.0, epsilon);
        }
    }
}
