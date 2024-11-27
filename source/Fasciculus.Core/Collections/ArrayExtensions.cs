namespace System
{
    public static class ArrayExtensions
    {
        public static T[] ShallowCopy<T>(this T[] array)
        {
            T[] result = new T[array.Length];

            array.CopyTo(result, 0);

            return result;
        }

        public static T[] SubArray<T>(this T[] array, int start, int count)
        {
            T[] result = new T[count];

            Array.Copy(array, start, result, 0, count);

            return result;
        }
    }
}
