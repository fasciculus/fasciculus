using Microsoft.Win32;
using System.Collections.Generic;

namespace Fasciculus.Windows
{
    public class RegistryPath
    {
        private readonly string[] names;

        public RegistryHive Hive { get; }
        public IEnumerable<string> Names { get { return names; } }

        public RegistryPath(RegistryHive hive, params string[] names)
        {
            Hive = hive;
            this.names = [.. names];
        }
    }
}
