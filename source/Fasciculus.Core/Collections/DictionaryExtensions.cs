namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static void Replace<K, V>(this Dictionary<K, V> dictionary, K key, V value)
            where K : notnull
        {
            dictionary.Remove(key);
            dictionary.Add(key, value);
        }
    }
}
