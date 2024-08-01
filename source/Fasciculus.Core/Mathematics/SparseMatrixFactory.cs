using Fasciculus.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Mathematics
{
    public class SparseMatrixFactory<T>
    {
        public class Key : ComparablePair<int, int>
        {
            public Key(int row, int column)
                : base(row, column) { }

            public static Key Create(int row, int column)
                => new(row, column);

            public int Row => First;
            public int Column => Second;
        }

        public class Entry : IComparable<Entry>
        {
            private readonly Key key;

            public int Row => key.Row;
            public int Column => key.Column;
            public readonly T Value;

            public Entry(Key key, T value)
            {
                this.key = key;
                Value = value;
            }

            public static Entry Create(KeyValuePair<Key, T> kvp)
                => new(kvp.Key, kvp.Value);

            public int CompareTo(Entry other)
                => key.CompareTo(other.key);
        }

        public readonly int RowCount;
        public readonly int ColumnCount;

        private readonly Dictionary<Key, T> values = new();

        private Entry[] Entries
            => values.Select(Entry.Create).OrderBy(e => e).ToArray();

        public SparseMatrixFactory(int rowCount, int columnCount)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
        }

        public void Set(int row, int column, T value)
        {
            values[Key.Create(row, column)] = value;
        }

        protected void BuildComponents(out int[] offsets, out int[] columns, out T[] values)
        {
            Entry[] entries = Entries;
            int count = entries.Length;
            int row = 0;

            offsets = new int[RowCount + 1];
            columns = new int[count];
            values = new T[count];

            for (int offset = 0; offset < count; offset++)
            {
                Entry entry = entries[offset];

                for (; row < entry.Row; ++row)
                {
                    offsets[row] = offset;
                }

                columns[offset] = entry.Column;
                values[offset] = entry.Value;
                ++offset;
            }

            for (++row; row <= RowCount; ++row)
            {
                offsets[row] = count;
            }
        }
    }
}
