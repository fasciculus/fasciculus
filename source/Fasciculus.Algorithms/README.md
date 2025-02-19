# Fasciculus.Algorithms

This package includes a bunch of algorithms.

## Array Pools

Invocation of actions or functions on pooled arrays.

## Array Equality and Comparison

The classes `ByteArrayEqualityComparer` and `ByteArrayComparer` allow testing for equality or
comparing of byte arrays.

## Searching

The class `BinarySearch` supports fast search in arrays of type `int` or `uint`.

Create an [Proposal](https://github.com/fasciculus/fasciculus/issues/new?template=02_api_proposal.yml),
if you need support for additional base types.

## Edit Distance

The class `EditDistance` calculates the edit distance between two arrays of arbitrary type.

## # Fuzzy Byte Arrays

The class `FuzzyBytes` converts an existing starting byte array into a sequence of byte arrays
that have an edit distance to the start array up to a given maximum.
