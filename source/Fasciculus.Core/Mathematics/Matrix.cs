using Fasciculus.Collections;
using Fasciculus.Validating;
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

    public abstract class Matrix<T, M, V>
        where T : notnull
        where M : Matrix<T, M, V>
        where V : Vector<T, V>
    {
        public int RowCount { get; }
        public int ColumnCount { get; }

        protected Matrix(int rowCount, int columnCount)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
        }

        public abstract T Get(int row, int column);

        public abstract M Add(M rhs);

        public static M operator +(Matrix<T, M, V> lhs, Matrix<T, M, V> rhs)
            => lhs.Add((M)rhs);

        public abstract V Mul(V vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static V operator *(Matrix<T, M, V> lhs, V rhs)
            => lhs.Mul(rhs);

        public abstract M Transpose();
    }

    public class SparseBoolMatrix : Matrix<bool, SparseBoolMatrix, SparseBoolVector>
    {
        private readonly int[] columns;
        private readonly int[] offsets;

        private readonly SparseBoolVector[] rows;

        private SparseBoolMatrix(int rowCount, int columnCount, int[] columns, int[] offsets)
            : base(rowCount, columnCount)
        {
            this.columns = columns;
            this.offsets = offsets;

            rows = Enumerable.Range(0, rowCount)
                .Select(row => SparseBoolVector.Create(BitSet.Create(columns, offsets[row], offsets[row + 1] - offsets[row])))
                .ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Get(int row, int column)
            => rows[row][column];

        public override SparseBoolMatrix Add(SparseBoolMatrix matrix)
        {
            throw Ex.NotImplemented();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override SparseBoolVector Mul(SparseBoolVector vector)
            => SparseBoolVector.Create(Enumerable.Range(0, RowCount).Where(index => rows[index] * vector));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SparseBoolVector operator *(SparseBoolMatrix matrix, SparseBoolVector vector)
            => matrix.Mul(vector);

        public override SparseBoolMatrix Transpose()
        {
            throw Ex.NotImplemented();
        }

        public static SparseBoolMatrix Create(int rowCount, int columnCount, IEnumerable<MatrixKey> entries)
            => Create(rowCount, columnCount, new SortedSet<MatrixKey>(entries));

        public static SparseBoolMatrix Create(int rowCount, int columnCount, SortedSet<MatrixKey> entries)
        {
            int[] columns = new int[entries.Count];
            int[] offsets = new int[rowCount + 1];
            int offset = 0;
            int lastRow = -1;

            foreach (MatrixKey entry in entries)
            {
                int currentRow = entry.Row;

                while (lastRow < currentRow)
                {
                    offsets[++lastRow] = offset;
                }

                columns[offset++] = entry.Column;
            }

            while (lastRow < rowCount)
            {
                offsets[++lastRow] = offset;
            }

            return new(rowCount, columnCount, columns, offsets);
        }
    }

    public class DenseIntMatrix : Matrix<int, DenseIntMatrix, DenseIntVector>
    {
        private readonly DenseIntVector[] rows;

        public DenseIntMatrix(int columnCount, DenseIntVector[] rows)
            : base(rows.Length, columnCount)
        {
            this.rows = rows.ShallowCopy();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int Get(int row, int column)
            => rows[row][column];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override DenseIntMatrix Add(DenseIntMatrix rhs)
            => new(ColumnCount, Enumerable.Range(0, RowCount).Select(row => rows[row] + rhs.rows[row]).ToArray());

        public override DenseIntVector Mul(DenseIntVector vector)
        {
            throw Ex.NotImplemented();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override DenseIntMatrix Transpose()
            => new(RowCount, Enumerable.Range(0, ColumnCount).Select(Transpose).ToArray());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DenseIntVector Transpose(int col)
            => new(Enumerable.Range(0, RowCount).Select(row => rows[row][col]).ToArray());
    }
}
