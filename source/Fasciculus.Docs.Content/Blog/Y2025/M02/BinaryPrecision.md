---
Title: "Binary Precision in Approximation Algorithms"
Published: 2025-02-01
Author: "Roger H. Jörg"
Summary: "How many bits are precise in a fixed point approximation algorithm."
---
# Binary Precision

!frontmatter{Author, Published}

While researching the division algorithm for my fixed point library,
I stumbled over the following formula that gives the count of correct bits $p_i$
after $i$ iterations of the approximation algorithm with an initial
error $0 \lt \epsilon_0 \lt 1$:

$$
\begin{align}
\notag p_i &= -2^i \log_2 \epsilon_0 - 1 \\
\notag p_i &= 2^i \log_2 (1/\epsilon_0) - 1
\end{align}
$$

Since a non-integral $p$ doesn't make much sense, I rewrite this as:

$$
\tag{1} p_i = 2^i \lfloor\log_2 (1/\epsilon_0)\rfloor - 1
$$

The $2^i$ element in the formula stems from the fact that the Newton iteration step
used in that division algorithm squares the error with each iteration
such that $\epsilon_{i+1} = \epsilon_i^2$, doubling the amount of correct bits.

Using $a \log(b) = \log b^a$, (1) can be rewritten as:

$$
\begin{align}
\notag p_i &= 2^i \lfloor\log_2 (1/\epsilon_0)\rfloor - 1 \\
\notag p_i &= \lfloor\log_2 (1/\epsilon_0^{2^i})\rfloor - 1
\end{align}
$$

This leads to a general function for any $\epsilon \gt 0$:

$$
\tag{2} p = \lfloor\log_2 (1/\epsilon)\rfloor - 1
$$

The formula seems ok, but why?

## Required Number of Bits

Let's look at some values $v$ and the required bit count $c$ to store them:

| $v$ | Binary | $c$ | $\log_2 v$ | &nbsp; | $v$ | Binary | $c$ | $\log_2 v$ | &nbsp; | $v$ | Binary | $c$ | $\log_2 v$ | &nbsp; | $v$ | Binary | $c$ | $\log_2 v$ |
| ---: | ---: | ---: | ---: | --- | ---: |  ---: | ---: | ---: | --- | ---: | ---: | ---: |  ---: | --- | ---: | ---: | ---: |  ---: |
| 0 | 0 | 0 | &nbsp; | &nbsp; | 5 | 101 | 3 | 2.322 | &nbsp; | 10 | 1010 | 4 | 3.322 | &nbsp; | 15 | 1111 | 4 | 3.907 |
| 1 | 1 | 1 | 0.000 | &nbsp; | 6 | 110 | 3 | 2.585 | &nbsp; | 11 | 1011 | 4 | 3.459 | &nbsp; | 16 | 10000 | 5 | 4.000 |
| 2 | 10 | 2 | 1.000 | &nbsp; | 7 | 111 | 3 | 2.807 | &nbsp; | 12 | 1100 | 4 | 3.585 | &nbsp; | 17 | 10001 | 5 | 4.087 |
| 3 | 11 | 2 | 1.585 | &nbsp; | 8 | 1000 | 4 | 3.000 | &nbsp; | 13 | 1101 | 4 | 3.700 | &nbsp; | 18 | 10010 | 5 | 4.167 | 
| 4 | 100 | 3 | 2.000 | &nbsp; | 9 | 1001 | 4 | 3.167 | &nbsp; | 14 | 1110 | 4 | 3.807 | &nbsp; | 19 | 10011 | 5 | 4.248 |

As you may already know, the increase in the required amount of bits happens when the value
$v$ is a power of two.

Looking at the colums with the logarithms, the bit count $c$ can be calculated as:

$$
\tag{3} c = \lfloor\log_2 v\rfloor + 1 \qquad\forall v \gt 0
$$

## Errors and Precision

Errors $\epsilon$ in approximation algorithms are usually given as fraction
(with $d \gt 0$):

$$
\epsilon = 1 / d
$$

As an example, the initial error in the aforementioned algorithm is
$\epsilon_0 = \frac{1}{17}$.

For a converging approximation algorithm, $\epsilon_0 \lt 1$ and therefore $d_0 \gt 1$.
After each iteration $\epsilon_{i+1} \lt \epsilon_i$ and therefore $d_{i+1} \gt d_i$.

For an $N$-bit mantissa of any such algorithm, the worst-case absolute error $\Epsilon$ is

$$
\begin{align}
\notag \Epsilon &= 2^N\epsilon \\
\notag \Epsilon &= \frac{2^N}{d}
\end{align}
$$

Using (3) to calculate the number of incorrect bits $q$:

$$
\begin{align}
\notag q &= \lceil \log_2(E) \rceil \\
\notag q &= \bigg\lceil \log_2\bigg(\frac{2^N}{d}\bigg) \bigg\rceil \\
\notag q &= \lceil \log_2(2^N) - \log_2(d) \rceil \\
\notag q &= N - \lceil \log_2(d) \rceil \\
\end{align}
$$

Being conservative*, this can be rewritten as

$$
q = N - \lfloor \log_2(d) \rfloor + 1 \\
$$

\* $q$ is too large by 1, if $d$ is a power of 2.

This gives the number of correct bits $p$ as

$$
\begin{align}
\notag p &= N - q \\
\notag p &= N - (N - \lfloor \log_2(d) \rfloor + 1) \\
\notag p &= \lfloor \log_2(d) \rfloor - 1 \\
\tag{4} p &= \lfloor \log_2(1/\epsilon) \rfloor - 1
\end{align}
$$

Now (4) is the same as (2). Q.E.D.