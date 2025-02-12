namespace Fasciculus.Mathematics.FixedPoint
{
    /// <summary>
    /// Common algorithms.
    /// </summary>
    public static class FP
    {
        /// <summary>
        /// Counts non-zero bits of the given <paramref name="value"/>.
        /// </summary>
        public static int CNZ8(byte value)
        {
            int x = (value & 0xAA) >> 1;
            int y = value & 0x55;
            int z = x + y;

            x = (z & 0xCC) >> 2;
            y = z & 0x33;
            z = x + y;

            x = (z & 0xF0) >> 4;
            y = z & 0x0F;

            z = x + y;

            return z;
        }

        /// <summary>
        /// Counts the leading zero bits of the given <paramref name="value"/>.
        /// </summary>
        public static int CLZ32(uint value)
        {
            if (value == 0) return 32;

            int count = 0;

            if ((value & 0xFFFF0000) == 0) { count += 16; value <<= 16; }
            if ((value & 0xFF000000) == 0) { count += 8; value <<= 8; }
            if ((value & 0xF0000000) == 0) { count += 4; value <<= 4; }
            if ((value & 0xC0000000) == 0) { count += 2; value <<= 2; }
            if ((value & 0x80000000) == 0) { count += 1; }

            return count;
        }
    }
}
