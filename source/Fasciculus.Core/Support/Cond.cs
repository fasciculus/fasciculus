using System;

namespace Fasciculus.Support
{
    public static class Cond
    {
        public static T NotNull<T>(T? value)
            where T : notnull
        {
            return value ?? throw new InvalidOperationException();
        }

        public static void IsNull<T>(T? value)
            where T : notnull
        {
            if (value is not null) throw new InvalidOperationException();
        }
    }
}
