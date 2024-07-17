using Fasciculus.IO;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.Eve.Models
{
    public static class Names
    {
        private static Dictionary<int, string> names = new();

        public static string Get(int id)
        {
            return names.TryGetValue(id, out var name) ? name : "";
        }

        public static void Set(int id, string name)
        {
            names[id] = name;
        }

        public static void Load(Stream stream)
        {
            Data data = new(stream);
            int count = data.ReadInt();

            for (int i = 0; i < count; i++)
            {
                int id = data.ReadInt();
                string name = data.ReadString();

                Set(id, name);
            }
        }
    }
}
