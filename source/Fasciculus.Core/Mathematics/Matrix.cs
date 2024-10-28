using Fasciculus.Collections;
using Fasciculus.Validating;
using System;
using System.Collections.Generic;

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

        public static MatrixKey Create(int row, int column)
        {
            return new MatrixKey(row, column);
        }

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

        public override bool Equals(object obj)
            => obj is MatrixKey mk && Equals(mk);

        public override int GetHashCode()
            => Row.GetHashCode() + Column.GetHashCode();

        public static bool operator ==(MatrixKey left, MatrixKey right)
            => left.Equals(right);
        public static bool operator !=(MatrixKey left, MatrixKey right)
            => !left.Equals(right);

        public static bool operator <(MatrixKey left, MatrixKey right)
            => left.CompareTo(right) < 0;
        public static bool operator >(MatrixKey left, MatrixKey right)
            => left.CompareTo(right) > 0;

        public static bool operator <=(MatrixKey left, MatrixKey right)
            => left.CompareTo(right) <= 0;
        public static bool operator >=(MatrixKey left, MatrixKey right)
            => left.CompareTo(right) >= 0;
    }

    public interface IMatrix<T>
        where T : notnull
    {
        public int RowCount { get; }
        public int ColumnCount { get; }

        public T Get(int row, int column);

        public Vector<T> Mul(Vector<T> vector);
    }

    public interface IMutableMatrix<T> : IMatrix<T>
        where T : notnull
    {
        public void Set(int row, int col, T value);

        public IMatrix<T> ToMatrix();
    }

    public abstract class Matrix<T> : IMatrix<T>
        where T : notnull
    {
        public int RowCount { get; }
        public int ColumnCount { get; }

        protected Matrix(int rowCount, int columnCount)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
        }

        public abstract T Get(int row, int column);

        public abstract Vector<T> Mul(Vector<T> vector);
    }

    public class SparseBoolMatrix : Matrix<bool>
    {
        private readonly int[] columns;
        private readonly int[] offsets;

        private SparseBoolMatrix(int rowCount, int columnCount, int[] columns, int[] offsets)
            : base(rowCount, columnCount)
        {
            this.columns = columns;
            this.offsets = offsets;
        }

        public override bool Get(int row, int column)
        {
            int index = offsets[row];
            int count = offsets[row + 1] - index;

            return Arrays.BinarySearch(columns, index, count, column) >= 0;
        }

        public override Vector<bool> Mul(Vector<bool> vector)
        {
            List<int> result = [];

            for (int row = 0; row < RowCount; ++row)
            {
                int lo = offsets[row];
                int hi = offsets[row + 1];

                for (int offset = lo; offset < hi; ++offset)
                {
                    int col = columns[offset];

                    if (vector[col])
                    {
                        result.Add(row);
                        break;
                    }
                }
            }

            return SparseBoolVector.Create(result);
        }

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

    public class MutableSparseBoolMatrix : Matrix<bool>, IMutableMatrix<bool>
    {
        private readonly SortedSet<MatrixKey> entries = [];

        public MutableSparseBoolMatrix(int rowCount, int columnCount)
            : base(rowCount, columnCount) { }

        public override bool Get(int row, int column)
            => entries.Contains(MatrixKey.Create(row, column));

        public void Set(int row, int col, bool value)
        {
            if (value)
            {
                entries.Add(MatrixKey.Create(row, col));
            }
            else
            {
                entries.Remove(MatrixKey.Create(row, col));
            }
        }

        public override Vector<bool> Mul(Vector<bool> vector)
        {
            throw Ex.NotImplemented();
        }

        public IMatrix<bool> ToMatrix()
            => SparseBoolMatrix.Create(RowCount, ColumnCount, entries);
    }

    public class DenseIntMatrix : Matrix<int>
    {
        private readonly int[] values;

        private DenseIntMatrix(int rowCount, int columnCount, int[] values)
            : base(rowCount, columnCount)
        {
            this.values = values;
        }

        public override int Get(int row, int column)
        {
            return values[row * ColumnCount + column];
        }

        public override Vector<int> Mul(Vector<int> vector)
        {
            throw Ex.NotImplemented();
        }

        internal static DenseIntMatrix Create(int rowCount, int columnCount, int[] values)
            => new(rowCount, columnCount, values.ShallowCopy());
    }

    public class MutableDenseIntMatrix : Matrix<int>, IMutableMatrix<int>
    {
        private readonly int[] values;

        public MutableDenseIntMatrix(int rowCount, int columnCount)
            : base(rowCount, columnCount)
        {
            values = new int[rowCount * columnCount];
        }

        public override int Get(int row, int column)
        {
            return values[row * ColumnCount + column];
        }


        public void Set(int row, int column, int value)
        {
            values[row * ColumnCount + column] = value;
        }

        public override Vector<int> Mul(Vector<int> vector)
        {
            throw Ex.NotImplemented();
        }

        public IMatrix<int> ToMatrix()
            => DenseIntMatrix.Create(RowCount, ColumnCount, values);

        public static MutableDenseIntMatrix Create(int rowCount, int columnCount)
            => new(rowCount, columnCount);
    }

    public static class Matrices
    {
        public static IMutableMatrix<bool> CreateMutableSparseBool(int rowCount, int columnCount)
            => new MutableSparseBoolMatrix(rowCount, columnCount);
    }
}
