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

        public T Get(int row, int col);
    }

    public interface IMutableMatrix<T> : IMatrix<T>
        where T : notnull
    {
        public void Set(int row, int col, T value);
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

        public abstract T Get(int row, int col);
    }

    public class MutableSparseBoolMatrix : Matrix<bool>, IMutableMatrix<bool>
    {
        private readonly SortedSet<MatrixKey> keys = [];

        public MutableSparseBoolMatrix(int rowCount, int columnCount)
            : base(rowCount, columnCount) { }

        public override bool Get(int row, int col)
            => keys.Contains(MatrixKey.Create(row, col));

        public void Set(int row, int col, bool value)
        {
            if (value)
            {
                keys.Add(MatrixKey.Create(row, col));
            }
            else
            {
                keys.Remove(MatrixKey.Create(row, col));
            }
        }
    }

    public class MutableDenseIntMatrix : Matrix<int>, IMutableMatrix<int>
    {
        private readonly int[] values;

        public MutableDenseIntMatrix(int rowCount, int columnCount)
            : base(rowCount, columnCount)
        {
            values = new int[rowCount * columnCount];
        }

        public override int Get(int row, int col)
        {
            return values[row * ColumnCount + col];
        }


        public void Set(int row, int col, int value)
        {
            values[row * ColumnCount + col] = value;
        }
    }

    public static class Matrices
    {
        public static IMutableMatrix<bool> CreateMutableSparseBool(int rowCount, int columnCount)
            => new MutableSparseBoolMatrix(rowCount, columnCount);

        public static IMutableMatrix<int> CreateMutableDenseInt(int rowCount, int columnCount)
            => new MutableDenseIntMatrix(rowCount, columnCount);
    }
}
