using System;
using System.Reflection;

namespace Fasciculus.Reflection
{
    /// <summary>
    /// Extensions for <see cref="object"/>.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Returns a property value of the given object
        /// </summary>
        public static T? GetRequiredProperty<T>(this object obj, string name)
            where T : class
        {
            Type type = obj.GetType();
            PropertyInfo? propertyInfo = type.GetProperty(name);
            object? value = propertyInfo?.GetValue(obj);

            return value as T;
        }
    }
}
