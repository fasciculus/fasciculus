using System.Collections.Generic;
using System.Text;

namespace Fasciculus.Text
{
    /// <summary>
    /// Provides conversion to Roman representation of numbers. 
    /// </summary>
    public static class RomanNumbers
    {
        private static readonly string[] romans = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
        private static readonly List<int> values = [1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1];

        /// <summary>
        /// Formats the given number into it's Roman representation.
        /// <para>Values less than 1 return an empty string. </para>
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The Roman representation.</returns>
        public static string Format(int value)
        {
            StringBuilder result = new();

            while (value > 0)
            {
                int index = values.FindIndex(x => x <= value);

                value -= values[index];
                result.Append(romans[index]);
            }

            return result.ToString();
        }
    }
}
