using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Algorithms
{
    /// <summary>
    /// A generator to create byte arrays fuzzied from a given start array
    /// </summary>
    public class FuzzyBytesGenerator
    {
        private readonly int maxDistance;

        private readonly SortedSet<byte[]> visited = new(ByteArrayComparer.Instance);

        private Queue<byte[]> currentQueue = [];
        private int currentDistance;

        private Queue<byte[]> nextQueue = [];

        private FuzzyBytesGenerator(byte[] start, int maxDistance)
        {
            this.maxDistance = maxDistance;

            visited.Add(start);
            currentQueue.Enqueue(start);
        }

        private IEnumerable<FuzzyBytes> Generate()
        {
            ++currentDistance;

            while (currentDistance <= maxDistance)
            {
                while (currentQueue.Count > 0)
                {
                    foreach (FuzzyBytes fuzzy in Generate(currentQueue.Dequeue()))
                    {
                        yield return fuzzy;
                    }
                }

                currentQueue = nextQueue;
                nextQueue = [];
                ++currentDistance;
            }
        }

        private IEnumerable<FuzzyBytes> Generate(byte[] start)
        {
            return Insert(start).Concat(Remove(start)).Concat(Replace(start));
        }

        private IEnumerable<FuzzyBytes> Insert(byte[] start)
        {
            for (int i = 0, n = start.Length; i <= n; ++i)
            {
                for (int j = 0; j < 256; ++j)
                {
                    byte[] bytes = new byte[n + 1];

                    Array.Copy(start, 0, bytes, 0, i);
                    bytes[i] = (byte)j;
                    Array.Copy(start, i, bytes, i + 1, n - i);

                    if (visited.Add(bytes))
                    {
                        nextQueue.Enqueue(bytes);

                        yield return new(bytes, currentDistance);
                    }
                }
            }
        }

        private IEnumerable<FuzzyBytes> Remove(byte[] start)
        {
            int n = start.Length;

            if (n > 0)
            {
                if (n == 1)
                {
                    byte[] bytes = [];

                    if (visited.Add(bytes))
                    {
                        nextQueue.Enqueue(bytes);

                        yield return new(bytes, currentDistance);
                    }
                }
                else
                {
                    for (int i = 0; i < n; ++i)
                    {
                        byte[] bytes = new byte[n - 1];

                        Array.Copy(start, 0, bytes, 0, i);
                        Array.Copy(start, i + 1, bytes, i, n - i - 1);

                        if (visited.Add(bytes))
                        {
                            nextQueue.Enqueue(bytes);

                            yield return new(bytes, currentDistance);
                        }
                    }
                }
            }
        }

        private IEnumerable<FuzzyBytes> Replace(byte[] start)
        {
            for (int i = 0, n = start.Length; i < n; ++i)
            {
                for (int j = 0; j < 256; ++j)
                {
                    byte[] bytes = new byte[n];

                    Array.Copy(start, 0, bytes, 0, i);
                    bytes[i] = (byte)j;
                    Array.Copy(start, i + 1, bytes, i + 1, n - i - 1);

                    if (visited.Add(bytes))
                    {
                        nextQueue.Enqueue(bytes);

                        yield return new(bytes, currentDistance);
                    }
                }
            }
        }

        /// <summary>
        /// Generates fuzzy bytes from the given start value.
        /// </summary>
        public static IEnumerable<FuzzyBytes> Generate(ReadOnlySpan<byte> start, int maxDistance)
        {
            FuzzyBytesGenerator fuzzyBytes = new(start.ToArray(), maxDistance);

            return fuzzyBytes.Generate();
        }
    }
}
