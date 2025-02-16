# Fasciculus.NetStandard.IO

This source-only package contains sources for IO operations not supported by `netstandard2.0`.
All sources are attributed with `internal`.

> To use this in a `netstandard2.0` project you need to reference the package `System.Memory`.

## Stream Extensions

- `Read(Span<byte> buffer)`
- `ReadExactly(Span<byte> buffer)`
- `ReadExactly(byte[] buffer, int offset, int count)`
- `ReadAtLeast(Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true)`

