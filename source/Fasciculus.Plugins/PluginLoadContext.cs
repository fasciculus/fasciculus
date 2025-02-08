using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Fasciculus.Plugins
{
    public class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver resolver;

        public PluginLoadContext(FileInfo pluginFile)
            : base(true)
        {
            resolver = new(pluginFile.FullName);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            string? path = resolver.ResolveAssemblyToPath(assemblyName);

            return path is null ? null : LoadFromAssemblyPath(path);
        }

        protected override nint LoadUnmanagedDll(string unmanagedDllName)
        {
            string? path = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            return path is null ? IntPtr.Zero : LoadUnmanagedDllFromPath(path);
        }
    }
}
