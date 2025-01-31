using System;
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
        public const ushort PosInf = 0x4000; // binary: 0100_0000_0000_0000

        /// <summary>
        /// Negative infinity.
        /// </summary>
        public const ushort NegInf = 0xC000; // binary: 1100_0000_0000_0000

        /// <summary>
        /// 1.0
        /// </summary>
        public const ushort One = 0x0100; // binary: 0000_0001_0000_0000

        /// <summary>
        /// -1.0
        /// </summary>
        public const ushort NegOne = 0x8100; // binary: 1000_0001_0000_0000

        /// <summary>
        /// Minimum value.
        /// </summary>
        public const ushort MinVal = 0xA000; // binary: 1010_0000_0000_0000

        /// <summary>
        /// Maximum value.
        /// </summary>
        public const ushort MaxVal = 0x2000; // binary: 0010_0000_0000_0000

        /// <summary>
        /// Smallest positive value greater than zero.
        /// </summary>
        public const ushort Eps = 0x0001; // binary: 0000_0000_0000_0001

        /// <summary>
        /// Mask for sign-bit.
        /// </summary>
        public const ushort SgnMsk = 0x8000; // binary: 1000_0000_0000_0000

        /// <summary>
        /// Mask for exceptional-bit.
        /// </summary>
        public const ushort ExcMsk = 0x4000; // binary: 0100_0000_0000_0000

        /// <summary>
        /// Mask for mantissa.
        /// </summary>
        public const ushort MntMsk = 0x3FFF; // binary: 0011_1111_1111_1111

        private const int MaxIntVal = 32 << 8;
        private const int MinIntVal = -MaxIntVal;

        /// <summary>
        /// Whether the given <paramref name="value"/> represents a <c>NaN</c>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNaN(ushort value)
            => (value & ExcMsk) != 0 && (value & MntMsk) > 0;

        /// <summary>
        /// Whether the given <paramref name="value"/> represents a negative value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNeg(ushort value)
            => !IsNaN(value) && IsNegUnsafe(value);

        /// <summary>
        /// Whether the given <paramref name="value"/> represents a negative value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegUnsafe(ushort value)
            => (value & SgnMsk) != 0;

        /// <summary>
        /// Whether the given <paramref name="value"/> represents a positive infinity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPosInf(ushort value)
            => value == PosInf;

        /// <summary>
        /// Whether the given <paramref name="value"/> represents a negative infinity.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegInf(ushort value)
            => value == NegInf;

        /// <summary>
        /// Whether the given <paramref name="value"/> represents a infinity, positive or negative.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInf(ushort value)
            => value == PosInf || value == NegInf;

        /// <summary>
        /// Whether the given <paramref name="value"/> represents zero value.
        /// </summary>
        public static bool IsZero(ushort value)
            => value == 0 || value == SgnMsk;

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
            => (ushort)(value & MntMsk);

        /// <summary>
        /// Returns the negative of the given <paramref name="value"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Neg(ushort value)
            => IsNaN(value) ? value : NegUnsafe(value);

        /// <summary>
        /// Returns the negative of the given <paramref name="value"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort NegUnsafe(ushort value)
            => (ushort)(value ^ SgnMsk);

        /// <summary>
        /// Converts the given <paramref name="value"/> to a <c>double</c>.
        /// </summary>
        public static double ToDouble(ushort value)
        {
            if (IsNaN(value)) return double.NaN;
            if (IsPosInf(value)) return double.PositiveInfinity;
            if (IsNegInf(value)) return double.NegativeInfinity;

            double result = (value & MntMsk) / 256.0;

            return IsNegUnsafe(value) ? -result : result;
        }

        /// <summary>
        /// Converts the given <paramref name="value"/> to a FP16Q8.
        /// </summary>
        public static ushort FromDouble(double value)
        {
            if (double.IsNaN(value)) return NaN;
            if (double.IsPositiveInfinity(value)) return PosInf;
            if (double.IsNegativeInfinity(value)) return NegInf;

            if (value > MaxIntVal) return PosInf;
            if (value < MinIntVal) return NegInf;

            bool neg = value < 0;
            ushort mantissa = (ushort)Math.Round(value * 256);

            return neg ? (ushort)(mantissa | SgnMsk) : mantissa;
        }

        /// <summary>
        /// Adds the given values <paramref name="lhs"/> and <paramref name="rhs"/>.
        /// </summary>
        public static ushort Add(ushort lhs, ushort rhs)
        {
            if (IsNaN(lhs) || IsNaN(rhs)) return NaN;
            if (IsInf(lhs)) return lhs;
            if (IsInf(rhs)) return rhs;

            return AddUnsafe(lhs, rhs);
        }

        /// <summary>
        /// Adds the given values <paramref name="lhs"/> and <paramref name="rhs"/>.
        /// </summary>
        public static ushort AddUnsafe(ushort lhs, ushort rhs)
        {
            bool lhsNeg = IsNegUnsafe(lhs);
            bool rhsNeg = IsNegUnsafe(rhs);

            int lhsInt = lhs & MntMsk;
            int rhsInt = rhs & MntMsk;

            lhsInt = lhsNeg ? -lhsInt : lhsInt;
            rhsInt = rhsNeg ? -rhsInt : rhsInt;

            int result = lhsInt + rhsInt;

            if (result < MinIntVal) return NegInf;
            if (result > MaxIntVal) return PosInf;

            bool resultNeg = result < 0;

            result = resultNeg ? -result : result;

            ushort mantissa = (ushort)(result & MntMsk);

            return resultNeg ? (ushort)(mantissa | SgnMsk) : mantissa;
        }

        /// <summary>
        /// Subtracts <paramref name="rhs"/> from <paramref name="lhs"/>.
        /// </summary>
        public static ushort Sub(ushort lhs, ushort rhs)
        {
            if (IsNaN(lhs) || IsNaN(rhs)) return NaN;
            if (IsInf(lhs)) return lhs;
            if (IsInf(rhs)) return rhs;

            return SubUnsafe(lhs, rhs);
        }

        /// <summary>
        /// Subtracts <paramref name="rhs"/> from <paramref name="lhs"/>.
        /// </summary>
        public static ushort SubUnsafe(ushort lhs, ushort rhs)
        {
            bool lhsNeg = IsNegUnsafe(lhs);
            bool rhsNeg = IsNegUnsafe(rhs);

            int lhsInt = lhs & MntMsk;
            int rhsInt = rhs & MntMsk;

            lhsInt = lhsNeg ? -lhsInt : lhsInt;
            rhsInt = rhsNeg ? -rhsInt : rhsInt;

            int result = lhsInt - rhsInt;

            if (result < MinIntVal) return NegInf;
            if (result > MaxIntVal) return PosInf;

            bool resultNeg = result < 0;

            result = resultNeg ? -result : result;

            ushort mantissa = (ushort)(result & MntMsk);

            return resultNeg ? (ushort)(mantissa | SgnMsk) : mantissa;
        }

        /// <summary>
        /// Multiplies <paramref name="lhs"/> with <paramref name="rhs"/>
        /// </summary>
        public static ushort Mul(ushort lhs, ushort rhs)
        {
            if (IsNaN(lhs) || IsNaN(rhs)) return NaN;

            if (IsInf(lhs) || IsInf(rhs))
            {
                bool lhsNeg = IsNegUnsafe(lhs);
                bool rhsNeg = IsNegUnsafe(rhs);

                return lhsNeg == rhsNeg ? PosInf : NegInf;
            }

            return MulUnsafe(lhs, rhs);
        }

        /// <summary>
        /// Multiplies <paramref name="lhs"/> with <paramref name="rhs"/>
        /// </summary>
        public static ushort MulUnsafe(ushort lhs, ushort rhs)
        {
            bool lhsNeg = IsNegUnsafe(lhs);
            bool rhsNeg = IsNegUnsafe(rhs);
            bool resultNeg = lhsNeg != rhsNeg;

            int lhsInt = lhs & MntMsk;
            int rhsInt = rhs & MntMsk;

            int absResult = (lhsInt * rhsInt) >> 8;
            int result = resultNeg ? absResult : -absResult;

            if (result < MinIntVal) return NegInf;
            if (result > MaxIntVal) return PosInf;

            ushort mantissa = (ushort)(absResult & MntMsk);

            return resultNeg ? (ushort)(mantissa | SgnMsk) : mantissa;
        }
    }
}
