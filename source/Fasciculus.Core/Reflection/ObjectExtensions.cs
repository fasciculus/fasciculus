using Fasciculus.Support;
using System.Reflection;

namespace System
{
    /// <summary>
    /// Extensions for <see cref="object"/>.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Returns a property value of the given object
        /// </summary>
        public static T GetRequiredProperty<T>(this object obj, string name)
            where T : class
        {
            Type type = obj.GetType();
            PropertyInfo propertyInfo = Cond.NotNull(type.GetProperty(name));
            object value = Cond.NotNull(propertyInfo.GetValue(obj));

            return Cond.NotNull(value as T);
        }
    }
}
