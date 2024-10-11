using Fasciculus.Collections;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Windows
{
    public class RegistryInfo
    {
        private readonly RegistryPath[] children;
        private readonly Dictionary<string, RegistryValue> values;

        public RegistryPath Path { get; }
        public bool Exists { get; }

        public IEnumerable<RegistryPath> Children => children;
        public IReadOnlyDictionary<string, RegistryValue> Values => values;

        public RegistryInfo(RegistryPath path)
        {
            using DisposableStack<RegistryKey> keys = new();
            RegistryKey parent = RegistryHelper.GetBaseKey(path.Hive);
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
                values = parent.GetValueNames().Select(name => RegistryValue.Create(parent, name)).ToDictionary(rk => rk.Name);
            }
            else
            {
                children = [];
                values = [];
            }
        }
    }
}
