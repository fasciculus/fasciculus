---
title: "Fixed Point Sinus Approximation"
publishedx: 2025-02-01
author: "Roger H. Jörg"
summary: "How to approximate trigonometric functions with fixed point"
---
# Fixed Point Sinus Approximation

My (future) fixed point library provides representations for angles. The
classes are named `AFP<N>` with $N \in \{8, 16, 32, 64\}$. They use the
respective underlying $N$-bit signed integer type (`sbyte`, `short`,
`int`, `long`) to store the angle value in $m$. All of these represent
the angle as

$$
\phi = 2\pi \frac{m}{2^N}
$$

Therefore an `AFP8` stored as an `sbyte` with $m \in [-128, 127]$
can represent 256 angles with $\phi \in [-\pi, \pi)$ (note: $\pi$ is not
included) and a "step size" of $\frac{2\pi}{256}$.

Obviously angles and some arithmetics do not make much sense unless
trigonometric algorithms are added. Due to the relations between them
(e.g. $\cos(\phi) = \sin(\phi + \pi/2)$), I will focus on Sinus.

## Return Type

Since $\sin(\phi) \in [-1, 1]$ the return type will be `FP<N>Q<N>`.
These types are less precise than the `AFP<N>` types as the value represented in them is

$$
x = -1^s m 2^{-(N - 3)} \in [-1, 1]
$$

with two bits required for house-keeping, one bit used to provide symmetry around $0$ and a "step size"
of $\frac{1}{2^{N - 3}}$ (e.g. $\frac{1}{32}$ for FP8Q8).

## Approximation Candidates

### Power/Taylor Series

### Angle Sum Identities

$$
\sin(\alpha + \beta) = \sin(\alpha)\cos(\beta) + \cos(\alpha)\sin(\beta)
$$

### Table Lookup and Interpolation

### Cordic Algorithm

### Convert to/from `double`

For `AFP<N>` with $N \in \{8, 16, 32\}$ 