using Microsoft.Win32;
using System;

namespace Fasciculus.Windows
{
    public class RegistryValue
    {
        public string Name { get; }
        public RegistryValueKind Kind { get; }

        private string? stringValue;
        private uint? uintValue;

        public string StringValue => stringValue ?? string.Empty;

        protected RegistryValue(string name, RegistryValueKind kind)
        {
            Name = name;
            Kind = kind;
        }

        protected RegistryValue(string name, RegistryValueKind kind, string value)
        {
            Name = name;
            Kind = kind;
            stringValue = value;
        }

        protected RegistryValue(string name, RegistryValueKind kind, uint value)
        {
            Name = name;
            Kind = kind;
            uintValue = value;
        }

        public static RegistryValue Create(RegistryKey key, string name)
        {
            RegistryValueKind kind = key.GetValueKind(name);
            object? value = key.GetValue(name);

            switch (kind)
            {
                case RegistryValueKind.None: return new(name, kind);
                case RegistryValueKind.Unknown: return new(name, kind);
                case RegistryValueKind.String: return new(name, kind, Convert.ToString(value));
                case RegistryValueKind.ExpandString: return new(name, kind, Convert.ToString(value));
                case RegistryValueKind.Binary: return new(name, kind);
                case RegistryValueKind.DWord: return new(name, kind, Convert.ToUInt32(value));
                case RegistryValueKind.MultiString: return new(name, kind);
                case RegistryValueKind.QWord: return new(name, kind);
            }

            return new(name, kind);
        }
    }
}
