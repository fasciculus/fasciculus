namespace Fasciculus.Mathematics
{
    public interface IMatrix<T>
    {
        public int RowCount { get; }
        public int ColumnCount { get; }
    }

    public interface IMutableMatrix<T> : IMatrix<T>
    {
        public void Set(int row, int col, T value);
    }

    public class MutableDenseIntMatrix : IMutableMatrix<int>
    {
        private readonly int[] values;

        public int RowCount { get; }
        public int ColumnCount { get; }

        public MutableDenseIntMatrix(int rowCount, int columnCount)
        {
            values = new int[rowCount * columnCount];

            RowCount = rowCount;
            ColumnCount = columnCount;
        }

        public void Set(int row, int col, int value)
        {
            values[row * ColumnCount + col] = value;
        }
    }

    public static class Matrices
    {
        public static IMutableMatrix<int> CreateMutableDenseInt(int rows, int cols)
            => new MutableDenseIntMatrix(rows, cols);
    }
}
