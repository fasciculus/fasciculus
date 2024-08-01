namespace Fasciculus.Mathematics
{
    public class SparseMatrixInt : SparseMatrix<int>
    {
        public SparseMatrixInt(int[] offsets, int[] columns, int[] values)
            : base(offsets, columns, values) { }
    }
}
