using Microsoft.Win32;
using System.Collections.Generic;

namespace Fasciculus.Windows.WindowsRegistry
{
    public class RegistryPath
    {
        private readonly string[] names;

        public RegistryHive Hive { get; }
        public IEnumerable<string> Names => names;
        public string Name => names.Length > 0 ? names[^1] : string.Empty;

        public RegistryPath(RegistryHive hive, params string[] names)
        {
            Hive = hive;
            this.names = [.. names];
        }

        public RegistryPath Combine(string name)
            => Combine(this, name);

        public static RegistryPath Combine(RegistryPath path, string name)
        {
            return new(path.Hive, [.. path.names, name]);
        }
    }
}
