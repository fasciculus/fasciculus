namespace Fasciculus.Collections
{
    public static class Arrays
    {
        public static int BinarySearch(int[] array, int index, int count, int value)
        {
            int lo = index;
            int hi = index + count - 1;

            while (lo <= hi)
            {
                int mid = lo + ((hi - lo) >> 1);
                int x = array[mid];

                if (x == value)
                {
                    return mid;
                }

                if (x < value)
                {
                    lo = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }

            return -1;
        }
    }
}
