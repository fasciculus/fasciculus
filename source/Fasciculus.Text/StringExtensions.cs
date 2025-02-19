using System;

namespace Fasciculus.Text
{
    /// <summary>
    /// Extensions for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns the edit distance between the two strings.
        /// </summary>
        public static int EditDistance(this string text, ReadOnlySpan<char> other)
            => Algorithms.Comparing.EditDistance.GetDistance(text.AsSpan(), other);
    }
}
