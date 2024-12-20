using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Windows.WindowsRegistry
{
    public class RegistryValues
    {
        private readonly Dictionary<string, RegistryValue> values;

        public static readonly RegistryValues Empty = new([]);

        protected RegistryValues(Dictionary<string, RegistryValue> values)
        {
            this.values = values;
        }

        public bool Contains(string name)
            => values.ContainsKey(name) && values[name].IsValid;

        public RegistryValue GetValue(string name)
            => values.TryGetValue(name, out RegistryValue? value) ? value : RegistryValue.Empty(name);

        public string GetString(string name)
            => GetValue(name).StringValue;

        public uint GetUInt(string name)
            => GetValue(name).UIntValue;

        public ulong GetULong(string name)
            => GetValue(name).ULongValue;

        public static RegistryValues Read(RegistryKey key)
            => new(key.GetValueNames().Select(name => RegistryValue.Read(key, name)).ToDictionary(rv => rv.Name));
    }
}
