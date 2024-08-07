﻿namespace Fasciculus.Mathematics
{
    public class SparseMatrixFactoryInt : SparseMatrixFactory<int>
    {
        public SparseMatrixFactoryInt(int rowCount, int columnCount)
            : base(rowCount, columnCount) { }

        public SparseMatrixFactoryInt(int count)
            : this(count, count) { }

        public SparseMatrixInt Build()
        {
            BuildComponents(out int[] offsets, out int[] columns, out int[] values);

            return new(offsets, columns, values);
        }
    }
}
