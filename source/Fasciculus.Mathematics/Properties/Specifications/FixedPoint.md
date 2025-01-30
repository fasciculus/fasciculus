# Fixed Point Algorithms

> [!WARNING]
> This is work-in-progress. It isn't even a draft yet.

The Fasciculus.Mathematics package will provide fixed point algorithms.

## §1 Terminology

**§1.1** As the Fasciculus.Mathematics package is a C# library, C# data types are used to
denote integers: `sbyte` or `byte` for 8-bit values, `short` or `ushort` for 16-bit
values, `int` or `uint` for 32-bit values and `long` or `ulong` for 64-bit values.

## §2 Types

The algorithms are static methods of various classes. The class names follow the pattern
`FP<total-bits>Q<fraction-bits>` for regular fixed point values and
`AFP<total-bits>` for angular values.

**§2.1** `<total-bits>` is the number of bits used to store a fixed point value. Throughout
the remainder of this document it is written as $N$ with $N\in\set{8, 16, 32, 64}_\N$

**§2.2** `<fraction-bits>` is the number of bits used to store the fractional part of a fixed
point value. Throughout the remainder of this document it is written as
$Q$ with $Q\in\set{1, 2, 3, ..., 64}_\N$. For $Q$ the following restrictions apply:

$$
\begin{cases}
Q = N &\text{for value range }[-1, 1]\\
Q \le N - 3 &\text{otherwise}
\end{cases}
$$

Regular fixed point values use the two most significant bits for house-keeping (see §3.2).

**§2.3** The $\epsilon$ value is the smallest representable value such that $x \plusmn\epsilon \ne x$.

Types where $N$ equals $Q$ have a larger $\epsilon$ than expected,
as again the two most significant bits are used for house-keeping and an additional bit is
used to provide symmetry around $0.0$.

Example types (of which not all will be implemented):

| Type | Range | $\epsilon$ | Usage |
| -- | -- | -- | -- |
| FP8Q8 | $[-1, 1]_\R$ | $2^{-5}$ | Neuronal Networks |
| FP16Q8 | $[-2^5, 2^5]_\R$ | $2^{-8}$ | Exhaustive testing |
| FP16Q16 | $[-1, 1]_\R$ | $2^{-13}$ | Exhaustive testing |
| FP32Q8 | $[-2^{21}, 2^{21}]_\R$ | $2^{-8}$ | |
| FP32Q16 | $[-2^{13}, 2^{13}]_\R$ | $2^{-16}$ | |
| FP32Q24 | $[-2^5, 2^5]_\R$ | $2^{-24}$ | |
| FP32Q32 | $[-1, 1]_\R$ | $2^{-29}$ | |
| FP64Q8 | $[-2^{53}, 2^{53}]_\R$ | $2^{-8}$ | |
| FP64Q16 | $[-2^{45}, 2^{45}]_\R$ | $2^{-16}$ | 3D-Engine |
| FP64Q32 | $[-2^{29}, 2^{29}]_\R$ | $2^{-32}$ | |
| FP64Q64 | $[-1, 1]_\R$ | $2^{-61}$ | 3D-Engine |
| AFP8 | $[-\pi, \pi - \epsilon ]_\R$ | $2\pi \times 2^{-8}$  | |
| AFP16 | $[-\pi, \pi - \epsilon ]_\R$ | $2\pi \times 2^{-16}$  | Exhaustive testing |
| AFP32 | $[-\pi, \pi - \epsilon ]_\R$ | $2\pi \times 2^{-32}$  | 3D-Engine |
| AFP64 | $[-\pi, \pi - \epsilon ]_\R$ | $2\pi \times 2^{-64}$  | |

Obviously way more types are possible with $Q$ values that are not a multiple of 8.

## §3 Storage and Interpretation

### §3.1 Underlying Type

**§3.1.1** `FP8` values are stored as `byte`, `FP16` as `ushort`, `FP32` as `uint`
and `FP64` as `ulong`.

**§3.1.2** `AFP8` values are stored as `sbyte`, `AFP16` as `short`, `AFP32` as `int`
and `AFP64` as `long`.

### §3.2 Bit numbering

The bits within the storage type used to store a fixed point value are numbered
from 0 (least significant bit or LSB) to $N{-}1$ (most significant bit or MSB).

### §3.3 Regular Fixed Point Values

**§3.3.1** Bit number $N{-}1$ holds the sign (denoted as $s$). If set, the represented value
is negative.

**§3.3.2** Bit number $N{-}2$ is the "exceptional" bit (denoted as $e$). If set, the
represented value is either infinity or `NaN` ("not-a-number").

**§3.3.3** The remaining bits (0 to $N{-}3$) form the mantissa (denoted as $m$).

#### §3.3.4 Positive Infinity

If only the $e$ bit is set, the represented value is $+\infty$.

#### §3.3.5 Negative Infinity

If both the $s$ bit and the $e$ bit are set and $m = 0$, the represented value is $-\infty$.

#### §3.3.7 Not-a-Number

If the $e$ bit is set and $m \ne 0$, the represented value is `NaN`.

#### §3.3.8 Represented Value

Unless the $e$ bit is set, the represented value is:

$$
x =
\begin{cases}
-1^s \times m \times 2^{-Q} &\text{if } Q < N\\
-1^s \times m \times 2^{-(Q-3)} &\text{if } Q = N
\end{cases}
$$

### §3.4 Angular Values

Due to the range of representable values, no infinity or `NaN` is needed. The mantissa
is interpreted as signed integer and the represented angle is:

$$
\phi = 2 \pi \times m \times 2^{-N}
$$

## §4 Contants

### §4.1 Regular Fixed Point Value Constants

#### §4.1.1 NaN Contants

All `FP` classes provide a constant representing `NaN`. Its binary value has all bits except
the $s$ bit set.

Example for `FP16Q8` and `FP16Q16`:

```cs
public const ushort NaN = 0x7FFF; // binary: 0111_1111_1111_1111
```

#### §4.1.2 Infinity Constants

All `FP` classes provide constants for $\plusmn\infty$. Its binary values have the
$s$ bit set according to the represented sign, the $e$ bit set to 1 and the $m$ bits
set to zero.

Example for `FP16Q8` and `FP16Q16`:

```cs
public const ushort PositiveInfinity = 0x4000; // binary: 0100_0000_0000_0000
public const ushort NegativeInfinity = 0xC000; // binary: 1100_0000_0000_0000
```

### §4.1.3 One Constants

All `FP` classes provide constants for $\plusmn1$.

Example for `FP16Q8`:

```cs
public const ushort One = 0x0100; // binary: 0000_0001_0000_0000
public const ushort NegativeOne = 0x8100; // binary: 1000_0001_0000_0000
```

Example for `FP16Q16`:

```cs
public const ushort One = 0x2000; // binary: 0010_0000_0000_0000
public const ushort NegativeOne = 0xA000; // binary: 1010_0000_0000_0000
```

### §4.1.4 Min and Max Value Constants

All `FP` classes where $Q \ne N$ provide constants for their minimum and maximum value.

Example for `FP16Q8`:

```cs
public const ushort MinValue = 0xA000; // binary: 1010_0000_0000_0000
public const ushort MaxValue = 0x2000; // binary: 0010_0000_0000_0000
```

### §4.1.4 Epsilon Constant

All `FP` classes provide constants for $\epsilon$.

Example for `FP16Q8` and `FP16Q16`:

```cs
public const ushort Epsilon = 0x0001; // binary: 0000_0000_0000_0001
```

### §4.2 Angular Fixed Point Value Constants

### §4.2.1 Constant for $\pi$

All `AFP` classes provide constants for $\pi$.

Example for `AFP16`:

```cs
public const short Pi = 0x8000; // binary: 1000_0000_0000_0000
```

### §4.2.2 Epsilon Constant

All `AFP` classes provide constants for $\epsilon$.

Example for `AFP16`:

```cs
public const short Epsilon = 0x0001; // binary: 0000_0000_0000_0001
```

## §5 Masks

### §5.1 Sign Mask

Example for `FP16Q8` and `FP16Q16`:

```cs
public static const ushort SignMask = 0x8000; // binary: 1000_0000_0000_0000
```

### §5.2 Exception Mask

Example for `FP16Q8` and `FP16Q16`:

```cs
public static const ushort ExceptionMask = 0x4000; // binary: 0100_0000_0000_0000
```

### §5.2 Mantissa Mask

Example for `FP16Q8` and `FP16Q16`:

```cs
public static const ushort MantissaMask = 0x3FFF; // binary: 0011_1111_1111_1111
```

## §6 Validation Check Algorithms

### $6.1 Check for `NaN`

All `FP` classes provide a ckeck for `NaN` named `IsNaN`.

Example for `FP16Q8` and `FP16Q16`:

```cs
public static bool IsNaN(ushort value);
```

### $6.1 Check for Infinity

All `FP` classes provide checks for infinity.

Example for `FP16Q8` and `FP16Q16`:

```cs
public static bool IsPositiveInfinity(ushort value);
public static bool IsNegativeInfinity(ushort value);
public static bool IsInfinity(ushort value); // positive or negative
```

## §7 Algorithm restrictions

### §7.1 Safe and Unsafe Algorithms

Most of the following algorithms come in two versions: a safe and an unsafe version.

The unsafe versions expect their arguments to be neither infinite nor `NaN`.

### §7.2 Overflow or Underflow

Most algorithms may return $+\infty$ on overflow or $-\infty$ on underflow.

### §7.3 Returning `NaN`

All algorithms return `NaN`, if at least on of their arguments is `NaN`.

Algorithms like division or square root return `NaN`, if the result is undefined
(like division by zero or square root of negative values).

## §8 Unary Algorithms

### §8.1 Negate

Toggles the $s$ bit of the value.

Example for `FP16Q8` and `FP16Q16`:

```cs
public static ushort Negate(ushort value);
public static ushort NegateUnsafe(ushort value);
```

### §8.2 Invert

Calculates the inverse ($1/x$) of the value.

Example for `FP16Q8` and `FP16Q16`:

```cs
public static ushort Invert(ushort value);
public static ushort InvertUnsafe(ushort value);
```

### 8.3 Conversion to `double`

Converts the value to a `double`. This may result in a loss of precision for
$N \gt 52$.

If the $e$ bit is set, this function returns the respective `double` counterpart.

Example for `FP16Q8` and `FP16Q16`:

```cs
public static double ToDouble(ushort value);
```

## §9 Shift Algorithms

### $9.1 Left Shift

Example for `FP16Q8` and `FP16Q16`:

```cs
public static ushort ShiftLeft(ushort value, uint count);
public static ushort ShiftLeftUnsafe(ushort value, uint count);
```

### $9.1 Right Shift

Example for `FP16Q8` and `FP16Q16`:

```cs
public static ushort ShiftRight(ushort value, uint count);
public static ushort ShiftRightUnsafe(ushort value, uint count);
```

## §10 Arithmetic Algorithms

### §10.1 Addition

Example for `FP16Q8` and `FP16Q16`:

```cs
public static ushort Add(ushort lhs, ushort rhs);
public static ushort AddUnsafe(ushort lhs, ushort rhs);
```

### §10.2 Subtraction

Example for `FP16Q8` and `FP16Q16`:

```cs
public static ushort Sub(ushort lhs, ushort rhs);
public static ushort SubUnsafe(ushort lhs, ushort rhs);
```

### §10.3 Multiplicaction

Example for `FP16Q8` and `FP16Q16`:

```cs
public static ushort Mul(ushort lhs, ushort rhs);
public static ushort MulUnsafe(ushort lhs, ushort rhs);
```

### §10.4 Division

Example for `FP16Q8` and `FP16Q16`:

```cs
public static ushort Div(ushort lhs, ushort rhs);
public static ushort DivUnsafe(ushort lhs, ushort rhs);
```

