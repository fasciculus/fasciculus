namespace System.Collections.Generic
{
    internal static partial class FasciculusDictionaryExtensions
    {
        internal static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);

                return true;
            }

            return false;
        }
    }
}