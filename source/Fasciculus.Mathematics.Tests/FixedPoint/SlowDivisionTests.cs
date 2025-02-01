using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Mathematics.Tests.FixedPoint
{
    [TestClass]
    public class SlowDivisionTests : TestsBase
    {
        [TestMethod]
        public void T0T1ForFP64Q16()
        {
            ulong u32 = 32 << 16;
            ulong u48 = 48 << 16;

            ulong t0 = Div(u48, 17);
            ulong t1 = Div(u32, 17);

            double t0x = t0 / 65536.0;
            double t1x = t1 / 65536.0;
            double e = 1.0 / 65536.0;

            Assert.AreEqual(48.0 / 17.0, t0x, e);
            Assert.AreEqual(32.0 / 17.0, t1x, e);
        }

        private static ulong Div(ulong n, ulong d)
        {
            ulong q = 0;
            ulong r = 0;

            for (int i = 63; i >= 0; --i)
            {
                ulong ni = (n >> i) & 1;

                r <<= 1;
                r |= ni;

                if (r >= d)
                {
                    r -= d;
                    q |= 1ul << i;
                }
            }

            if (r >= (d / 2))
            {
                ++q;
            }

            return q;
        }
    }
}
