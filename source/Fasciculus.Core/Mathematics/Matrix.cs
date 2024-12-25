using Fasciculus.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Fasciculus.Mathematics
{
    public struct MatrixKey : IEquatable<MatrixKey>, IComparable<MatrixKey>
    {
        public int Row { get; internal set; }
        public int Column { get; internal set; }

        public MatrixKey(int row, int column)
        {
            Row = row;
            Column = column;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MatrixKey Create(int row, int column)
            => new(row, column);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(MatrixKey other)
            => Row == other.Row && Column == other.Column;

        public int CompareTo(MatrixKey other)
        {
            int result = Row.CompareTo(other.Row);

            if (result == 0)
            {
                result = Column.CompareTo(other.Column);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj)
            => obj is not null && obj is MatrixKey mk && Equals(mk);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly int GetHashCode()
            => Row.GetHashCode() + Column.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(MatrixKey left, MatrixKey right)
            => left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(MatrixKey left, MatrixKey right)
            => !left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(MatrixKey left, MatrixKey right)
            => left.CompareTo(right) < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(MatrixKey left, MatrixKey right)
            => left.CompareTo(right) > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(MatrixKey left, MatrixKey right)
            => left.CompareTo(right) <= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(MatrixKey left, MatrixKey right)
            => left.CompareTo(right) >= 0;
    }

    public class SparseBoolMatrix
    {
        private readonly Dictionary<uint, SparseBoolVector> rows;
        private readonly SortedSet<uint> indices;

        public SparseBoolMatrix(Dictionary<uint, SparseBoolVector> rows)
        {
            this.rows = rows.Where(r => r.Value.Length()).ToDictionary();
            indices = new(this.rows.Keys);
        }

        public SparseBoolVector this[uint index]
            => rows.TryGetValue(index, out SparseBoolVector? row) ? row : SparseBoolVector.Empty;

        public static SparseBoolVector operator *(SparseBoolMatrix m, SparseBoolVector v)
            => new(m.indices.Where(i => m[i] * v));
    }

    public class SparseShortMatrix
    {
        private readonly Dictionary<uint, SparseShortVector> rows;
        private readonly SortedSet<uint> indices;

        public SparseShortVector this[uint index]
            => rows.TryGetValue(index, out SparseShortVector? row) ? row : SparseShortVector.Empty;

        public SparseShortMatrix(Dictionary<uint, SparseShortVector> rows)
        {
            this.rows = rows.Where(r => r.Value.Indices.Any()).ToDictionary();
            indices = new(this.rows.Keys);
        }

        public SparseShortMatrix(Binary bin)
        {
            rows = bin.ReadDictionary(bin.ReadUInt, () => new SparseShortVector(bin));
            indices = new(rows.Keys);
        }

        public void Write(Binary bin)
        {
            bin.WriteDictionary(rows, bin.WriteUInt, v => v.Write(bin));
        }
    }
}
