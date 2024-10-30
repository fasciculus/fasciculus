using System;

namespace Fasciculus.Collections
{
    public static class Arrays
    {
        public static T[] SubArray<T>(T[] array, int index, int count)
        {
            T[] result = new T[count];

            Array.Copy(array, index, result, 0, count);

            return result;
        }
    }
}
