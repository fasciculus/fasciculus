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
    }
}
