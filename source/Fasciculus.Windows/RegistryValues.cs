using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Windows
{
    public class RegistryValues
    {
        private readonly Dictionary<string, RegistryValue> values;

        public static readonly RegistryValues Empty = new(new());

        protected RegistryValues(Dictionary<string, RegistryValue> values)
        {
            this.values = values;
        }

        public bool Contains(string name)
            => values.ContainsKey(name) && values[name].IsValid;

        public string GetString(string name)
            => values.TryGetValue(name, out RegistryValue value) ? value.StringValue : string.Empty;

        public uint GetUInt(string name)
            => values.TryGetValue(name, out RegistryValue value) ? value.UIntValue : 0;

        public ulong GetULong(string name)
            => values.TryGetValue(name, out RegistryValue value) ? value.ULongValue : 0;

        public static RegistryValues Read(RegistryKey key)
            => new(key.GetValueNames().Select(name => RegistryValue.Read(key, name)).ToDictionary(rv => rv.Name));
    }
}
