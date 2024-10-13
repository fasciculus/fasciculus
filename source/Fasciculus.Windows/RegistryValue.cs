using Microsoft.Win32;
using System;

namespace Fasciculus.Windows
{
    public class RegistryValue
    {
        public string Name { get; }
        public RegistryValueKind Kind { get; }
        public bool IsValid { get; }

        private string? stringValue;
        private uint? uintValue;
        private ulong? ulongValue;

        public string StringValue => stringValue ?? string.Empty;
        public uint UIntValue => uintValue ?? 0;
        public ulong ULongValue => ulongValue ?? 0;

        protected RegistryValue(string name, RegistryValueKind kind)
        {
            Name = name;
            Kind = kind;
            IsValid = false;
        }

        protected RegistryValue(string name, RegistryValueKind kind, string value)
        {
            Name = name;
            Kind = kind;
            IsValid = true;
            stringValue = value;
        }

        protected RegistryValue(string name, RegistryValueKind kind, uint value)
        {
            Name = name;
            Kind = kind;
            IsValid = true;
            uintValue = value;
        }

        protected RegistryValue(string name, RegistryValueKind kind, ulong value)
        {
            Name = name;
            Kind = kind;
            IsValid = true;
            ulongValue = value;
        }

        public static RegistryValue Empty(string name)
            => new(name, RegistryValueKind.None);

        public static RegistryValue Read(RegistryKey key, string name)
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
                case RegistryValueKind.QWord: return new(name, kind, Convert.ToUInt64(value));
            }

            return new(name, kind);
        }
    }
}
