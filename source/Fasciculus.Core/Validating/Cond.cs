using System;

namespace Fasciculus.Validating
{
    public static class Cond
    {
        public static T NotNull<T>(T? value)
            where T : notnull
        {
            return value ?? throw new InvalidOperationException();
        }
    }
}
