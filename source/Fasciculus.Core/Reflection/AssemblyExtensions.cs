using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    /// <summary>
    /// <see cref="Assembly"/> extensions.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Returns the custom attribute of type <typeparamref name="T"/> of the given <paramref name="assembly"/>.
        /// </summary>
        /// <returns>
        /// The custom attribute or <c>null</c> if <paramref name="assembly"/> is <c>null</c> or no such attribute exists
        /// in the assembly.
        /// </returns>
        public static T? GetAttribute<T>(this Assembly? assembly)
            where T : Attribute
        {
            CustomAttributeData? data = assembly?.CustomAttributes
                .FirstOrDefault(x => x.AttributeType == typeof(T));

            if (data is null)
            {
                return null;
            }

            IEnumerable<object?> args = data.ConstructorArguments.Select(x => x.Value);
            object? attribute = data.Constructor.Invoke(args.ToArray());

            return attribute is not null && attribute is T result ? result : null;
        }

        /// <summary>
        /// Returns the value of the <see cref="AssemblyInformationalVersionAttribute"/> attribute of the given <paramref name="assembly"/>.
        /// <para>
        /// Resorts to <see cref="GetFileVersion(Assembly?)"/> if no such attribute exists.
        /// </para>
        /// </summary>
        public static string GetSemanticVersion(this Assembly? assembly)
        {
            return assembly?.GetAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? assembly?.GetFileVersion() ?? string.Empty;
        }

        /// <summary>
        /// Returns the file version of the given <paramref name="assembly"/>.
        /// </summary>
        public static string GetFileVersion(this Assembly? assembly)
        {
            AssemblyName? name = assembly?.GetName();
            Version? version = name?.Version;

            return version?.ToString() ?? string.Empty;
        }
    }
}
