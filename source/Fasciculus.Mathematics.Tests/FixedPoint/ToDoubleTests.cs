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
            double minValue = FP16Q8.ToDouble(FP16Q8.MinVal);
            double maxValue = FP16Q8.ToDouble(FP16Q8.MaxVal);
            double epsilon = FP16Q8.ToDouble(FP16Q8.Eps);

            Assert.AreEqual(-32.0, minValue);
            Assert.AreEqual(32.0, maxValue);
            Assert.AreEqual(1.0 / 256.0, epsilon);
        }
    }
}
