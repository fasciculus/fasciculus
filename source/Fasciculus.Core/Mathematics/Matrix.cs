using Fasciculus.Collections;
using Fasciculus.Validating;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public abstract class Matrix<T>
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

        public static Vector<T> operator *(Matrix<T> matrix, Vector<T> vector)
            => matrix.Mul(vector);
    }

    public abstract class MutableMatrix<T> : Matrix<T>
        where T : notnull
    {
        protected MutableMatrix(int rowCount, int columnCount)
            : base(rowCount, columnCount)
        {
        }
    }

    public class SparseBoolMatrix : Matrix<bool>
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

        public override bool Get(int row, int column)
            => rows[row][column];

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

        public BitSet Mul(BitSet vector)
        {
            List<int> result = [];

            for (int row = 0; row < RowCount; ++row)
            {
                int index = offsets[row];
                int count = offsets[row + 1] - index;
                BitSet bs = new(columns, index, count);

                if (bs.Intersects(vector))
                {
                    result.Add(row);
                }
            }

            return new BitSet([.. result], 0, result.Count);
        }

        public static BitSet operator *(SparseBoolMatrix matrix, BitSet vector)
            => matrix.Mul(vector);

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

    public class MutableSparseBoolMatrix : MutableMatrix<bool>
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

        public SparseBoolMatrix ToMatrix()
            => SparseBoolMatrix.Create(RowCount, ColumnCount, entries);

        public static MutableSparseBoolMatrix Create(int rowCount, int columnCount)
            => new(rowCount, columnCount);
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

    public class MutableDenseIntMatrix : MutableMatrix<int>
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

        public DenseIntMatrix ToMatrix()
            => DenseIntMatrix.Create(RowCount, ColumnCount, values.ShallowCopy());

        public static MutableDenseIntMatrix Create(int rowCount, int columnCount)
            => new(rowCount, columnCount);
    }
}
