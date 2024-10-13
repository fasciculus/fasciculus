using Fasciculus.Collections;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Windows
{
    public class RegistryInfo
    {
        private readonly RegistryPath[] children;
        private readonly RegistryValues values;

        public RegistryPath Path { get; }
        public bool Exists { get; }

        public IEnumerable<RegistryPath> Children => children;
        public RegistryValues Values => values;

        public RegistryInfo(RegistryPath path)
        {
            using DisposableStack<RegistryKey> keys = new();
            RegistryKey parent = RegistryHives.GetRegistryKey(path.Hive);
            bool exists = true;

            foreach (string name in path.Names)
            {
                RegistryKey? key = parent.OpenSubKey(name, false);

                if (key is null)
                {
                    exists = false;
                    break;
                }
                else
                {
                    keys.Push(key);
                    parent = key;
                }
            }

            Path = path;
            Exists = exists;

            if (exists)
            {
                children = parent.GetSubKeyNames().Select(name => RegistryPath.Combine(path, name)).ToArray();
                values = RegistryValues.Read(parent);
            }
            else
            {
                children = [];
                values = RegistryValues.Empty;
            }
        }

        public static RegistryInfo Read(RegistryPath path)
            => new(path);
    }
}
