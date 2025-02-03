using System;

namespace Fasciculus.Support
{
    /// <summary>
    /// Support for conditions.
    /// </summary>
    public static class Cond
    {
        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if the given <paramref name="value"/> is <c>null</c>.
        /// </summary>
        public static T NotNull<T>(T? value, string? message = null)
            where T : notnull
        {
            return value ?? throw Ex.InvalidOperation(message);
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if the given <paramref name="value"/> is not <c>null</c>.
        /// </summary>
        public static void IsNull<T>(T? value, string? message = null)
            where T : notnull
        {
            if (value is not null) Ex.InvalidOperation(message);
        }
    }
}
