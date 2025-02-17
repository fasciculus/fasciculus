#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Collections.Generic
#pragma warning restore IDE0130 // Namespace does not match folder structure
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