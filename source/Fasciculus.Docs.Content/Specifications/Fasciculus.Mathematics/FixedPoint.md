---
Title: "Fixed Point Algorithms"
Status: "pre-draft"
Version: "2025-02-12"
---
# Fixed Point Algorithms

!frontmatter{Status,Version}

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

#### §3.3.8 Zero

If the $e$ bit is not set and $m = 0$, the represented value is $0$. The $s$ bit
is ignored.

This results in the fact that $-0$ exists. It is treated as $0$.

#### §3.3.9 Represented Value

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

Value for `FP16Q8` and `FP16Q16`:

```cs
public const ushort NaN = 0x7FFF; // binary: 0111_1111_1111_1111
```

#### §4.1.2 Infinity Constants

All `FP` classes provide constants for $\plusmn\infty$. Its binary values have the
$s$ bit set according to the represented sign, the $e$ bit set to 1 and the $m$ bits
set to zero.

Values for `FP16Q8` and `FP16Q16`:

```cs
public const ushort PosInf = 0x4000; // binary: 0100_0000_0000_0000
public const ushort NegInf = 0xC000; // binary: 1100_0000_0000_0000
```

### §4.1.3 One Constants

All `FP` classes provide constants for $\plusmn1$.

Values for `FP16Q8`:

```cs
public const ushort One = 0x0100; // binary: 0000_0001_0000_0000
public const ushort NegOne = 0x8100; // binary: 1000_0001_0000_0000
```

Values for `FP16Q16`:

```cs
public const ushort One = 0x2000; // binary: 0010_0000_0000_0000
public const ushort NegOne = 0xA000; // binary: 1010_0000_0000_0000
```

### §4.1.4 Min and Max Value Constants

All `FP` classes where $Q \ne N$ provide constants for their minimum and maximum value.

Values for `FP16Q8`:

```cs
public const ushort MinVal = 0xA000; // binary: 1010_0000_0000_0000
public const ushort MaxVal = 0x2000; // binary: 0010_0000_0000_0000
```

### §4.1.4 Epsilon Constant

All `FP` classes provide constants for $\epsilon$.

Value for `FP16Q8` and `FP16Q16`:

```cs
public const ushort Eps = 0x0001; // binary: 0000_0000_0000_0001
```

### §4.2 Angular Fixed Point Value Constants

### §4.2.1 Constant for $\pi$

All `AFP` classes provide constants for $\pi$.

Value for `AFP16`:

```cs
public const short Pi = 0x8000; // binary: 1000_0000_0000_0000
```

### §4.2.2 Epsilon Constant

All `AFP` classes provide constants for $\epsilon$.

Value for `AFP16`:

```cs
public const short Eps = 0x0001; // binary: 0000_0000_0000_0001
```

## §5 Masks

### §5.1 Sign Mask

Value for `FP16Q8` and `FP16Q16`:

```cs
public const ushort SgnMsk = 0x8000; // binary: 1000_0000_0000_0000
```

### §5.2 Exception Mask

Value for `FP16Q8` and `FP16Q16`:

```cs
public const ushort ExcMsk = 0x4000; // binary: 0100_0000_0000_0000
```

### §5.2 Mantissa Mask

Value for `FP16Q8` and `FP16Q16`:

```cs
public const ushort MntMsk = 0x3FFF; // binary: 0011_1111_1111_1111
```

## §6 Validation Check Algorithms

### $6.1 Check for `NaN`

All `FP` classes provide a ckeck for `NaN` named `IsNaN`.

Signature for `FP16Q8` and `FP16Q16`:

```cs
public static bool IsNaN(ushort value);
```

### $6.2 Check for Negativity

Checks whether the given value is negative.

The result is `false` for `NaN`.

The result is `true` for $-\infty$, `false` for $\infty$.

The result is `false` for $0$.

Signatures for `FP16Q8` and `FP16Q16`:

```cs
public static bool IsNeg(ushort value);
public static bool IsNegUnsafe(ushort value);
```

### $6.3 Check for Infinity

All `FP` classes provide checks for infinity.

Signatures for `FP16Q8` and `FP16Q16`:

```cs
public static bool IsPosInf(ushort value);
public static bool IsNegInf(ushort value);
public static bool IsInf(ushort value); // positive or negative
```

### $6.3 Check for Zero

Signature for `FP16Q8` and `FP16Q16`:

```cs
public static bool IsZero(ushort value);
```

## §7 Algorithm restrictions

### §7.1 Safe and Unsafe Algorithms

Most algorithms come in two versions: a safe and an unsafe version.

The unsafe versions expect their arguments to not be `NaN`.

Some unsafe versions expect their arguments to not be infinite neither.

### §7.2 Overflow or Underflow

Most algorithms may return $+\infty$ on overflow or $-\infty$ on underflow.

### §7.3 Returning `NaN`

All safe algorithms return `NaN`, if at least on of their arguments is `NaN`.

Safe algorithms like division or square root return `NaN`, if the result is undefined
(like division by zero or square root of negative values).

## §8 Unary Algorithms

### §8.1 Absolute Value

The absolute value of `NaN` is `NaN`.

The absolute value of $-\infty$ is $\infty$.

`AbsUnsafe` expects its argument to not be `NaN`.

Signatures for `FP16Q8` and `FP16Q16`:

```cs
public static ushort Abs(ushort value);
public static ushort AbsUnsafe(ushort value);
```

### §8.2 Negate

Toggles the $s$ bit of the value.

Signatures for `FP16Q8` and `FP16Q16`:

`NegUnsafe` expects its argument to not be `NaN`.

```cs
public static ushort Neg(ushort value);
public static ushort NegUnsafe(ushort value);
```

### §8.3 Reciprocal ($1/x$)

Calculates the reciprocal ($1/x$) of the value.

The reciprocals of `NaN`, $\infty$, $-\infty$ and $0$ are all `NaN`.

`RecUnsafe` expects its argument to neither be `NaN`, infinite nor $0$.

Signatures for `FP16Q8` and `FP16Q16`:

```cs
public static ushort Rec(ushort value);
public static ushort RecUnsafe(ushort value);
```

### 8.4 Conversions

All conversions may result in a loss of precision.

#### 8.4.1 Conversion to `double`

This conversion may result in a loss of precision for $N \gt 52$.

If the $e$ bit is set, this function returns the respective `double` counterpart.

Signature for `FP16Q8` and `FP16Q16`:

```cs
public static double ToDouble(ushort value);
```

#### 8.4.1 Conversion from `double`

This conversion may result in a loss of precision for $N \lt 52$.

This conversion returns $\infty$ (or $-\infty$) if the given value doesn't fit into
the target type.

This conversion returns `NaN`, $\infty$ or $-\infty$ if the given value is one
of these values.

Signature for `FP16Q8` and `FP16Q16`:

```cs
public static ushort FromDouble(double value);
```

## §9 Shift Algorithms

### $9.1 Left Shift

Signatures for `FP16Q8` and `FP16Q16`:

```cs
public static ushort ShiftLeft(ushort value, uint count);
public static ushort ShiftLeftUnsafe(ushort value, uint count);
```

### $9.1 Right Shift

Signatures for `FP16Q8` and `FP16Q16`:

```cs
public static ushort ShiftRight(ushort value, uint count);
public static ushort ShiftRightUnsafe(ushort value, uint count);
```

## §10 Arithmetic Algorithms

### §10.1 Addition

Example for `FP16Q8` and `FP16Q16`:

`AddUnsafe` expects its arguments to neither be `NaN` nor infinite.

Signatures for `FP16Q8` and `FP16Q16`:

```cs
public static ushort Add(ushort lhs, ushort rhs);
public static ushort AddUnsafe(ushort lhs, ushort rhs);
```

### §10.2 Subtraction

Example for `FP16Q8` and `FP16Q16`:

`SubUnsafe` expects its arguments to neither be `NaN` nor infinite.

Signatures for `FP16Q8` and `FP16Q16`:

```cs
public static ushort Sub(ushort lhs, ushort rhs);
public static ushort SubUnsafe(ushort lhs, ushort rhs);
```

### §10.3 Multiplicaction

Example for `FP16Q8` and `FP16Q16`:

`MulUnsafe` expects its arguments to neither be `NaN` nor infinite.

Signatures for `FP16Q8` and `FP16Q16`:

```cs
public static ushort Mul(ushort lhs, ushort rhs);
public static ushort MulUnsafe(ushort lhs, ushort rhs);
```

### §10.4 Division

`DivUnsafe` expects its arguments to neither be `NaN` nor infinite.

`DivUnsafe` expects its secon argument (the divisor) to not be $0$.

Signatures for `FP16Q8` and `FP16Q16`:

```cs
public static ushort Div(ushort lhs, ushort rhs);
public static ushort DivUnsafe(ushort lhs, ushort rhs);
```

## §A Division Algorithms

For $N \gt 32$ the division algorithms are implemented as a multiplication of the nominator
$n$ with the reciprocal $r$ of the denominator $d$ to get the quotient $q$:

$$
\tag{A.1}
q = n / d = n (1 / d) = n r
$$

### §A.1 Preparations

To calculate the initial estimate of the Newton iterations, $d$ must be
in the range $[0.5, 1.0]$. This can be achieved as follows.

1. If $d \lt 0$, both $n$ and $d$ are negated.
2. If $d \notin [0.5, 1.0]$ both $n$ and $d$ are bit shifted:
    1. If $d \lt 0.5$, both $n$ and $d$ are left shifted by the appropriate number of bits.
    2. If $d \gt 1.0$, both $n$ and $d$ are right shifted by the appropriate number of bits.

Step 2.1. may overflow $n$, leading to a premature $\infty$ or $-\infty$ result.

Step 2.2. may lead to $n$ becoming $0$, leading to a premature $0$ result.

### §A.2 Newton Iteration Step

According to this [Wikipedia article][wiki_div], to find the inverse (reciprocal) $r$ of
$d$ we use Newton's method to solve the equation:

$$
\tag{A.2.1}
f(r) = (1/r) - d = r^{-1} - d = 0
$$

The derivative of (A.2.1) is:

$$
\tag{A.2.2}
f'(r) = -1 / r^2= -r^{-2}
$$

Starting from an initial estimate $r_0$, a Newton iteration is given as:

$$
\begin{align}
\notag r_{i+1} &= r_i - \frac{f(r_i)}{f'(r_i)} \\
\notag r_{i+1} &= r_i - \frac{r^{-1} - d}{-r^{-2}} \\
\notag r_{i+1} &= r_i + \frac{r^{-1} - d}{r^{-2}} \\
\notag r_{i+1} &= r_i + r_i^2(r^{-1} - d) \\
\tag{A.2.3} r_{i+1} &= r_i + r_i(1 - r_i d) \\
\tag{A.2.4} r_{i+1} &= r_i (2 - r_i d)
\end{align}
$$

The resulting iteration step uses addition, subtraction and multiplication only. For
comptational precision, (A.2.3) is preferred.

Since $d r = 1$ and $d r_i \approx 1$ the error $\epsilon_i$ is defined as:

$$
\notag    \epsilon_i = 1 - d r_i
$$

After an iteration the error is:

$$
\begin{align}
\notag    \epsilon_{i+1} &= 1 - d r_{i+1} \\
\notag    &= 1 - d (r_i (2 - r_i d)) \\
\notag    &= 1 - d (2 r_i - r_i^2 d) \\
\notag    &= 1 - 2 d r_i + r_i^2 d^2 \\
\notag    &= (1 - d r_i)^2 \\
\notag    &= \epsilon_i^2
\end{align}
$$

Given a good initial estimate $r_0$, such that |$\epsilon_0| \lt 1$, every iteration will
double the amount of correct digits (bits) of the result.

### §A.3 Initial Estimate

For the following to work, $d$ must be limited:

$$
d \in [0.5, 1.0]
$$

with the lower bound $d_0$ and the upper bound $d_1$:

$$
\begin{align}
\tag{A.3.1}    d_0 &= 0.5 = 1/2 \\
\tag{A.3.2}    d_1 &= 1
\end{align}
$$

The initial estimate $r_0$ is (second order Chebyshev polynomial):

$$
\notag  r_0 = t_0 + t_1 d \approx r
$$

Since $d r = 1$ and $d r_0 \approx 1$, the absolute value of the error $\epsilon_0$ is:

$$
\begin{align}
\notag  |\epsilon_0| &= |d r - d r_0| \\
\notag               &= |1 - d(t_0 + d t_1)|    
\end{align}
$$

This gives the error function and its derivative:

$$
\begin{align}
\tag{A.3.3}   f(d) &= 1 - d(t_0 + d t_1) \\
\tag{A.3.4}   f(d) &= 1 - t_0 d - t_1 d^2 \\
\notag        f'(d) &= -t_0 - 2 t_1 d
\end{align}
$$

To minimize the error $|\epsilon_0|$, the local minimum of $f(d)$ is required.
It occurs at $f'(d) = 0$. Solving for d:

$$
\begin{align}
\notag           f'(d) &= 0 \\
\notag  -t_0 - 2 t_1 d &= 0 \\
\notag        -2 t_1 d &= t_0 \\
\tag{A.3.5}          d &= - \frac{t_0}{2 t_1}
\end{align}
$$

Applying the Chebyshev equioscillation theorem gives the equations:

$$
\begin{align}
\tag{A.3.6}  f(d_0) &= f(d_1) \\
\notag       f(d_0) &= -f(d) \\
\tag{A.3.7}  f(d_1) &= -f(d)
\end{align}
$$

Using (A.3.6) with (A.3.4) to solve for $t_1$:

$$
\begin{align}
\notag  f(d_0) &= f(d_1) \\
\notag  1 - t_0 d_0 - t_1 d_0^2 &= 1 - t_0 d_1 - t_1 d_1^2 \\
\notag  1 - \frac{1}{2} t_0 - \frac{1}{4} t_1 &=  1 - t_0 - t_1 \\
\notag  4 - 2 t_0 - t_1 &= 4 - 4 t_0 - 4 t_1 \\
\notag  - 2 t_0 - t_1 &= - 4 t_0 - 4 t_1 \\
\notag  - t_1 &= - 2 t_0 - 4 t_1 \\
\notag  3 t_1 &= - 2 t_0 \\
\tag{A.3.8}  t_1 &= - \frac{2}{3} t_0
\end{align}
$$

Using (A.3.8) with (A.3.3) to define $f(d_1)$:

$$
\begin{align}
\notag       f(d_1) &= 1 - d_1(t_0 + d_1 t_1) \\
\notag       f(d_1) &= 1 - (t_0 + t_1) \\
\notag       f(d_1) &= 1 - \bigg(t_0 - \frac{2}{3} t_0\bigg) \\
\tag{A.3.9}  f(d_1) &= 1 - \frac{1}{3} t_0
\end{align}
$$

Using (A.3.5) and (A.3.8) to redefine (A.3.4):

$$
\begin{align}
\notag  f(d) &= 1 - t_0 d - t_1 d^2 \\
\notag  f(d) &= 1 - t_0 \bigg(- \frac{t_0}{2 t_1}\bigg) - t_1 \bigg(- \frac{t_0}{2 t_1}\bigg)^2 \\
\notag  f(d) &= 1 - t_0 \bigg(- \frac{t_0}{2 (- \frac{2}{3} t_0)}\bigg) - \bigg(- \frac{2}{3} t_0\bigg) \bigg(- \frac{t_0}{2 (- \frac{2}{3} t_0)}\bigg)^2 \\
\notag  f(d) &= 1 - \bigg(\frac{t_0^2}{\frac{4}{3} t_0}\bigg) - \bigg(- \frac{2}{3} t_0\bigg) \bigg(\frac{t_0}{\frac{4}{3} t_0}\bigg)^2 \\
\notag  f(d) &= 1 - \bigg(\frac{t_0}{\frac{4}{3}}\bigg) + \frac{2}{3} t_0 \bigg(\frac{1}{\frac{4}{3}}\bigg)^2 \\
\notag  f(d) &= 1 - \frac{3}{4}t_0 + \frac{2}{3} t_0 \bigg(\frac{1}{\frac{16}{9}}\bigg) \\
\notag  f(d) &= 1 - \frac{36}{48}t_0 + \frac{18}{48} t_0 \\
\tag{A.3.10}  f(d) &= 1 - \frac{9}{24}t_0  \\
\end{align}
$$

Using (A.3.7) with (A.3.9) and (A.3.10) to solve for $t_1$:

$$
\begin{align}
\notag  -f(d) &= f(d1) \\
\notag  -(1 - \frac{9}{24}t_0) &= 1 - \frac{1}{3} t_0 \\
\notag   \frac{9}{24}t_0 - 1 &= 1 - \frac{8}{24} t_0 \\
\notag   \frac{9}{24}t_0 &= 2 - \frac{8}{24} t_0 \\
\notag   \frac{17}{24}t_0 &= \frac{48}{24} \\
\notag   t_0 &= \frac{48}{17} \\
\notag   t_1 &= - \frac{2}{3} t_0 \\
\notag   t_1 &= - \frac{2}{3} \frac{48}{17} \\
\tag{A.3.11}   t_1 &= - \frac{32}{17} \\
\end{align}
$$

Using (A.3.8) and (A.3.11) to solve for $t_0$:

$$
\begin{align}
\notag - \frac{2}{3} t_0 &= t_1 \\
\notag - \frac{2}{3} t_0 &= - \frac{32}{17} \\
\tag{A.3.12} t_0 &= \frac{96}{34} = \frac{48}{17} \\
\end{align}
$$

The resulting initial estimate is therefore:

$$
\begin{align}
\notag r_0 &= t_0 + t_1 d \\
\tag{A.3.13} r_0 &= \frac{48}{17} - \frac{32}{17} d
\end{align}
$$

### §A.4 Required Newton Iterations

The error $\epsilon_0$ produced by these parameters is at most:

$$
\begin{align}
\notag  |\epsilon_0| &= |f(d_1)| \\
\notag  |\epsilon_0| &= \bigg|1 - \frac{1}{3} t_0\bigg| \\
\notag  |\epsilon_0| &= \bigg|1 - \frac{1}{3} \frac{48}{17}\bigg| \\
\notag  |\epsilon_0| &= \bigg|1 - \frac{16}{17}\bigg| \\
\notag  |\epsilon_0| &= \frac{1}{17} \\
\end{align}
$$

The number of bits affected (precise) at an error $\epsilon$ is:

$$
p = \lfloor - log_2 \epsilon \rfloor - 1
$$

This gives the following precisions $p$ after $i$ Newton iterations:

| $i$ | $\epsilon$ | $p$ | Satisfies |
| --- | --- | ---: | --- |
| 0 | $17^{-1}$ | 3 | |
| 1 | $17^{-2}$ | 7 | FP8 |
| 2 | $17^{-4}$ | 15 | FP16 |
| 3 | $17^{-8}$ | 31 | FP32, `float` |
| 4 | $17^{-16}$ | 64 | FP64, `double` |

Using a third-order Chebyshev polynomial for $r_0$:

$$
r_0 = t_0 + t_1 d + t_2 d^2
$$

which has an $\epsilon_0$ of $1/99$ doesn't reduce the number of iterations as $p$ is
$52$ for $i = 3$. That may satisfy `double` (52 bit mantissa) but not
`FP64` (62 bit mantissa).

[wiki_div]: https://en.wikipedia.org/wiki/Division_algorithm#Newton%E2%80%93Raphson_division