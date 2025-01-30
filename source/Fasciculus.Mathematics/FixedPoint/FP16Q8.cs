using System.Runtime.CompilerServices;

namespace Fasciculus.Mathematics.FixedPoint
{
    /// <summary>
    /// Fixed point N=16, Q=8
    /// </summary>
    public static class FP16Q8
    {
        /// <summary>
        /// Not-a-Number.
        /// </summary>
        public const ushort NaN = 0x7FFF; // binary: 0111_1111_1111_1111

        /// <summary>
        /// Positive infinity.
        /// </summary>
        public const ushort PositiveInfinity = 0x4000; // binary: 0100_0000_0000_0000

        /// <summary>
        /// Negative infinity.
        /// </summary>
        public const ushort NegativeInfinity = 0xC000; // binary: 1100_0000_0000_0000

        /// <summary>
        /// 1.0
        /// </summary>
        public const ushort One = 0x0100; // binary: 0000_0001_0000_0000

        /// <summary>
        /// -1.0
        /// </summary>
        public const ushort NegativeOne = 0x8100; // binary: 1000_0001_0000_0000

        /// <summary>
        /// Minimum value.
        /// </summary>
        public const ushort MinValue = 0xA000; // binary: 1010_0000_0000_0000

        /// <summary>
        /// Maximum value.
        /// </summary>
        public const ushort MaxValue = 0x2000; // binary: 0010_0000_0000_0000

        /// <summary>
        /// Smallest positive value greater than zero.
        /// </summary>
        public const ushort Epsilon = 0x0001; // binary: 0000_0000_0000_0001

        /// <summary>
        /// Mask for sign-bit.
        /// </summary>
        public const ushort SignMask = 0x8000; // binary: 1000_0000_0000_0000

        /// <summary>
        /// Mask for exceptional-bit.
        /// </summary>
        public const ushort ExceptionMask = 0x4000; // binary: 0100_0000_0000_0000

        /// <summary>
        /// Mask for mantissa.
        /// </summary>
        public const ushort MantissaMask = 0x3FFF; // binary: 0011_1111_1111_1111

        private const int MaxIntValue = 32 << 8;
        private const int MinIntValue = -MaxIntValue;

        /// <summary>
        /// Whether the given <paramref name="value"/> represents a <c>NaN</c>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNaN(ushort value)
            => (value & ExceptionMask) != 0 && (value & MantissaMask) > 0;

        /// <summary>
        /// Whether the given <paramref name="value"/> represents a nrgative value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative(ushort value)
            => !IsNaN(value) && IsNegativeUnsafe(value);

        /// <summary>
        /// Whether the given <paramref name="value"/> represents a nrgative value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegativeUnsafe(ushort value)
            => (value & SignMask) != 0;

        /// <summary>
        /// Whether the given <paramref name="value"/> represents a positive infinity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositiveInfinity(ushort value)
            => value == PositiveInfinity;

        /// <summary>
        /// Whether the given <paramref name="value"/> represents a negative infinity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegativeInfinity(ushort value)
            => value == NegativeInfinity;

        /// <summary>
        /// Whether the given <paramref name="value"/> represents a infinity, positive or negative.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInfinity(ushort value)
            => value == PositiveInfinity || value == NegativeInfinity;

        /// <summary>
        /// Returns the absolute value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Abs(ushort value)
            => IsNaN(value) ? NaN : AbsUnsafe(value);

        /// <summary>
        /// Returns the absolute value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort AbsUnsafe(ushort value)
            => (ushort)(value & MantissaMask);

        /// <summary>
        /// Returns the negative of the given <paramref name="value"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Negate(ushort value)
            => IsNaN(value) ? value : NegateUnsafe(value);

        /// <summary>
        /// Returns the negative of the given <paramref name="value"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort NegateUnsafe(ushort value)
            => (ushort)(value ^ SignMask);

        /// <summary>
        /// Converts the given <paramref name="value"/> to a <c>double</c>.
        /// </summary>
        public static double ToDouble(ushort value)
        {
            if (IsNaN(value)) return double.NaN;
            if (IsPositiveInfinity(value)) return double.PositiveInfinity;
            if (IsNegativeInfinity(value)) return double.NegativeInfinity;

            double result = (value & MantissaMask) / 256.0;

            return IsNegativeUnsafe(value) ? -result : result;
        }

        /// <summary>
        /// Adds the given values <paramref name="lhs"/> and <paramref name="rhs"/>.
        /// </summary>
        public static ushort Add(ushort lhs, ushort rhs)
        {
            if (IsNaN(lhs) || IsNaN(rhs)) return NaN;
            if (IsInfinity(lhs)) return lhs;
            if (IsInfinity(rhs)) return rhs;

            return AddUnsafe(lhs, rhs);
        }

        /// <summary>
        /// Adds the given values <paramref name="lhs"/> and <paramref name="rhs"/>.
        /// </summary>
        public static ushort AddUnsafe(ushort lhs, ushort rhs)
        {
            bool lhsNeg = IsNegativeUnsafe(lhs);
            bool rhsNeg = IsNegativeUnsafe(rhs);

            int lhsInt = lhs & MantissaMask;
            int rhsInt = rhs & MantissaMask;

            lhsInt = lhsNeg ? -lhsInt : lhsInt;
            rhsInt = rhsNeg ? -rhsInt : rhsInt;

            int result = lhsInt + rhsInt;

            if (result < MinIntValue) return NegativeInfinity;
            if (result > MaxIntValue) return PositiveInfinity;

            bool resultNeg = result < 0;

            result = resultNeg ? -result : result;

            ushort mantissa = (ushort)(result & MantissaMask);

            return resultNeg ? (ushort)(mantissa | SignMask) : mantissa;
        }
    }
}
