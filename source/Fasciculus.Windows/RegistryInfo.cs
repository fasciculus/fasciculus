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
            using RegistryStack stack = new();

            Path = path;
            Exists = stack.Open(path);

            if (Exists)
            {
                children = stack.Top.GetSubKeyNames().Select(name => RegistryPath.Combine(path, name)).ToArray();
                values = RegistryValues.Read(stack.Top);
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
