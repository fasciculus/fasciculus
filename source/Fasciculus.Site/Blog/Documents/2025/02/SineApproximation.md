---
title: "Fixed Point Sine Approximation"
publishedx: 2025-02-01
author: "Roger H. Jörg"
summary: "How to approximate trigonometric functions with fixed point"
---
# Fixed Point Sine Approximation

My (future) fixed point library provides representations for angles. The
classes are named `AFP<N>` with $N \in \{8, 16, 32, 64\}$. They use the
respective underlying $N$-bit signed integer type (`sbyte`, `short`,
`int`, `long`) to store the angle value in a mantissa $m$. All of these represent
the angle as

$$
\phi = 2\pi \frac{m}{2^N}
$$

Therefore an `AFP8` stored as an `sbyte` with $m \in [-128, 127]$
can represent 256 angles with $\phi \in [-\pi, \pi)$ (note: $\pi$ is not
included) and a "step size" of $2\pi / 256$.

Obviously angles and some arithmetics do not make much sense unless
trigonometric algorithms are added. Due to the relations between them
(e.g. $\cos(\phi) = \sin(\phi + \pi/2)$), I will focus on sine.

## Return Type

Since $\sin(\phi) \in [-1, 1]$ the return type will be `FP<N>Q<N>`, meaning the number of
bits used to represent the fractional part is maximized (actually $N-3$).
These types are less precise than the `AFP<N>` types. The value represented in
`FP<N>Q<N>` types is

$$
x = -1^s m 2^{-(N - 3)} \in [-1, 1]
$$

with two bits required for house-keeping, one bit used to provide symmetry around zero and a "step size" of $1/2^{N - 3}$ (e.g. $\frac{1}{32}$ for FP8Q8).

## Symmetries

The sine function is symmetric with respect to the point $\phi = 0$.

$$
\sin(-\phi) = -\sin(\phi)
$$

The sine function is symmetric with respect to the line $\phi = \pi/2$.

$$
\sin(\pi/2 + \phi) = \sin(\pi/2 - \phi)
$$

With the symmetries above, $\sin(0) = 0$ and $\sin(\pi/2) = 1$ the approximations can be
restricted to

$$
\phi \in \{x \,|\, 0 \lt x \lt \pi/2 \}_{\R}
$$

## Approximation Algorithm Candidates

### Taylor Series

$$
\begin{align}
\notag    \sin(\phi) &= \sum_{n = 0}^\infty (-1)^n \frac{x^{2n + 1}}{(2n+1)!} \\
\notag    &= \frac{x}{1!} - \frac{x^3}{3!} + \frac{x^5}{5!} \mp \ldots
\end{align}
$$

The algoritm requires the use of a larger data type (`FP64Q<?>`) to store the fast growing denominator
and the accumulating sum.

The algoritm terminates if the nominator becomes zero (due to available precision) or if the
denominator overflows becoming $\infty$ where I define (mathematically incorrect) $x/\infty = 0$.

The Taylor series are definitively required to precompute values for other approximations. More
on that in a other blog entry.

### Euler's Formula

$$
\begin{align}
\notag    e^{i\phi} &= \cos(\phi) + i \sin(\phi) \\
\notag    \sin(\phi) &= \text{Im} (e^{i\phi}) \\
\notag               &= \frac{e^{i\phi} - e^{-i\phi}}{2i}
\end{align}
$$

with $e^{i\phi}$ calculated using the MacLaurin Series

$$
e^{i\phi} = 1 + i\phi + \frac{(i\phi)^2}{2!} + \frac{(i\phi)^3}{3!} + \frac{(i\phi)^4}{4!} + \ldots
$$

Using an additional series doesn't really help, as I might just directly use the Taylor
series above. Therefore this approach isn't further investigated.

### Angle Sum Identities

$$
\sin(\alpha \plusmn \beta) = \sin(\alpha)\cos(\beta) \plusmn \cos(\alpha)\sin(\beta)
$$

The idea behind this algorithm is to start with $\alpha_0 = 0$ and $\beta_0 = \pi/4$ (that's 45°) and initial values $s_0 = \sin(\alpha_0) = 0$ and $c_0 = \cos(\alpha_0) = 1$.

Each iteration uses the identity above to calculate

$$
\begin{align}
\notag  s_{i+1} &=
\begin{cases}
    \sin(\alpha_i)\cos(\beta_i) + \cos(\alpha_i)\sin(\beta_i) \qquad \text{for } \alpha_i \lt \phi \\
    \sin(\alpha_i)\cos(\beta_i) - \cos(\alpha_i)\sin(\beta_i) \qquad \text{for } \alpha_i \gt \phi
\end{cases} \\
\notag  &=
\begin{cases}
    s_i\cos(\beta_i) + c_i\sin(\beta_i) \qquad \text{for } \alpha_i \lt \phi \\
    s_i\cos(\beta_i) - c_i\sin(\beta_i) \qquad \text{for } \alpha_i \gt \phi
\end{cases} \\
\notag c_{i+1} &= \sqrt{1 - (s_{i+1})^2} \\
\notag \alpha_{i+1}  &= 
\begin{cases}
    \alpha_i + \beta_i \qquad \text{for } \alpha_i \lt \phi \\
    \alpha_i - \beta_i \qquad \text{for } \alpha_i \gt \phi \\
\end{cases} \\
\notag \beta_{i+1} &= \beta_i / 2
\end{align}
$$

The $\alpha_i$ value approaches $\phi$ with each iteration.

The algorithm terminates when $\beta_i$ becomes zero due to the available precision.

The algorithm requires a lookup table for all the $\sin(\beta_i)$ and $\cos(\beta_i)$. The table
size is small with $n = \log_2(N) + 1$ (e.g. 4&nbsp;for `AFP8` or&nbsp;7 for `AFP64`).

The available precision of $\beta_i$ controls the required number of iterations, namely 4&nbsp;for
`AFP8` or 7&nbsp;for `AFP64`.

This algorithm has similarities with the Cordic algorithm (see below) which doesn't require a
square root. Therefore this approach isn't further investigated.

### Table Lookup and Interpolation

This algorithm uses two lookup tables of size $n+1$ defining the supporting points

$$
\phi_i \to \sin(\phi_i) \qquad \text{ for } i \in \{x\,|\,0 \le x \le n\}_{\N}
$$

The first table contains the $\phi_i$ values (sorted), the second table contains the
associated precomputed $\sin(\phi_i)$ values.

The tables include data points for $\phi = 0$ and $\phi = \pi/2$.

The values in the first table are equidistant meaning there is the value $D$ such that

$$
D = \phi_{i+1} - \phi_i \qquad \forall i \text{ with } 0 \le i \lt n
$$

To calculate $\sin(\phi)$, a binary search in the first table is used to find $\phi_i$
and $\phi_{i+1}$ such that $\phi_i \le \phi \lt \phi_{i+1}$.

If $\phi_i = \phi$ the algorithm terminates prematurely by just returning $\sin(\phi_i)$
from the second table. Otherwise an interpolation occurs.

For the interpolation the value $t$ is required

$$
\begin{align}
\notag t &= \frac{\Delta\phi}{D} \\
\notag &= \frac{\phi - \phi_i}{D}
\end{align}
$$

### Linear Interpolation (Lerp)

This is easy

$$
\sin(\phi) = \sin(\phi_i) + t(\sin(\phi_{i + 1}) - \sin(\phi_i)) \\
$$

where $\sin(\phi_i)$ and $\sin(\phi_{i + 1})$ are precomputed values from the second table.

This requires rather large tables.

### Higher Order (Spline) Interpolation (Slerp)

For this to work we need to know the steepness of the sine curve at $\sin(\phi_i)$ and
$\sin(\phi_{i + 1})$ which is defined as $\cos(\phi_i)$ and $\cos(\phi_{i + 1})$ respectively.
The cosines are precomputed and stored in a third table.

Quadratic splines and cubic splines are reasonable. They require smaller tables than the linear
interpolation. More on that in a later blog entry.

### Cordic Algorithm

The Cordic algorithm uses vector rotation to calculate its results. For sine (and cosine) we
start with a vector representing $\alpha_0 = 0$

$$
\vec{v_0} = \begin{bmatrix}x_0\\ y_0\end{bmatrix} = \begin{bmatrix}1\\ 0\end{bmatrix}
$$

A second angle is initialized with $\beta_0 = \pi/4$.

Each iteration
1. rotates $\vec{v_i}$ by $\beta_i$ with a precomputed rotation matrix $R_i$ towards $\phi$.
2. increases or decreases $\alpha_i$ by $\beta_i$ accordingly (i.e. towards $\phi$).
3. approximately halves $\beta$ such that $\beta_{i+1} \approx \beta_i / 2$.

Once $\beta_i$ is small enough (zero due to the available precision) $x_i = \sin(\phi)$
and $y_i = \cos(\phi)$.

The algorithm uses addition, subtraction, multiplication and shift operations only and makes
it definitively onto the list of algorithms to investigate. More on that in a later blog entry.

### Convert to/from `double`

For `AFP<N>` with $N \in \{8, 16, 32\}$ the $\phi$ could just be converted to a `double` $x$,
then $y = \sin(\phi)$ is calculated using `y = Math.Sin(x)`. Finally $y$ is converted back to the
result type.

This doesn't work for `AFP64` due to the lack of precision of a `double`. But it may be the
solution to go for the smaller types.