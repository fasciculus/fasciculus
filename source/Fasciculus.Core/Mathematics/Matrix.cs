using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Fasciculus.Mathematics
{
    public readonly struct MatrixKey : IEquatable<MatrixKey>, IComparable<MatrixKey>
    {
        public int Row { get; init; }
        public int Column { get; init; }

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
        public override bool Equals(object obj)
            => obj is MatrixKey mk && Equals(mk);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
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
        public int RowCount => rows.Length;
        public int ColumnCount { get; }

        private readonly SparseBoolVector[] rows;

        public SparseBoolMatrix(int columnCount, SparseBoolVector[] rows)
        {
            ColumnCount = columnCount;
            this.rows = rows.ShallowCopy();
        }

        public SparseBoolVector this[int row]
            => rows[row];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SparseBoolVector operator *(SparseBoolMatrix matrix, SparseBoolVector vector)
            => SparseBoolVector.Create(Enumerable.Range(0, matrix.RowCount).Where(index => matrix.rows[index] * vector));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SparseBoolMatrix Create(int rowCount, int columnCount, IEnumerable<MatrixKey> entries)
            => new(columnCount, Enumerable.Range(0, rowCount).Select(row => CreateRow(row, entries)).ToArray());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SparseBoolVector CreateRow(int row, IEnumerable<MatrixKey> entries)
            => SparseBoolVector.Create(entries.Where(e => e.Row == row).Select(e => e.Column));
    }

    public class DenseIntMatrix
    {
        public int RowCount => rows.Length;
        public int ColumnCount { get; }

        private readonly DenseIntVector[] rows;

        public DenseIntMatrix(int columnCount, DenseIntVector[] rows)
        {
            ColumnCount = columnCount;
            this.rows = rows.ShallowCopy();
        }

        public DenseIntVector this[int row]
            => rows[row];

        public static DenseIntMatrix operator +(DenseIntMatrix lhs, DenseIntMatrix rhs)
            => new(lhs.ColumnCount, Enumerable.Range(0, lhs.RowCount).Select(row => lhs.rows[row] + rhs.rows[row]).ToArray());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DenseIntMatrix Transpose()
            => new(RowCount, Enumerable.Range(0, ColumnCount).Select(Transpose).ToArray());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DenseIntVector Transpose(int col)
            => new(Enumerable.Range(0, RowCount).Select(row => rows[row][col]).ToArray());
    }
}
