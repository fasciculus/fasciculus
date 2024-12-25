using Fasciculus.Support;
using System;
using System.Linq;
using System.Reflection;

namespace Fasciculus.IO.Resources
{
    /// <summary>
    /// Utility class to find embedded resources.
    /// </summary>
    public static class EmbeddedResources
    {
        /// <summary>
        /// Finds an embedded resource in the assemblies loaded in <see cref="AppDomain.CurrentDomain"/>.
        /// </summary>
        public static EmbeddedResource Find(string name)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string[] names = assembly.GetManifestResourceNames();

                if (names.Contains(name))
                {
                    return new EmbeddedResource(assembly, name);
                }
            }

            throw Ex.ResourceNotFound(name);
        }
    }
}
