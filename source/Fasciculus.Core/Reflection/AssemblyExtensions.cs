using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    public static class AssemblyExtensions
    {
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

        public static string GetFileVersion(this Assembly? assembly)
        {
            AssemblyName? name = assembly?.GetName();
            Version? version = name?.Version;

            return version?.ToString() ?? string.Empty;
        }

        public static string GetSemanticVersion(this Assembly? assembly)
        {
            return assembly?.GetAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? assembly?.GetFileVersion() ?? string.Empty;
        }
    }
}
