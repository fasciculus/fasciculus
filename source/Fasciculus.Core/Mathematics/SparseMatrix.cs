namespace Fasciculus.Mathematics
{
    public class SparseMatrix<T>
    {
        protected readonly int[] offsets;
        protected readonly int[] columns;
        protected readonly T[] values;

        public SparseMatrix(int[] offsets, int[] columns, T[] values)
        {
            this.offsets = offsets;
            this.columns = columns;
            this.values = values;
        }
    }
}
