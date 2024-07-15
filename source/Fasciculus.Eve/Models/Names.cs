using System.Collections.Generic;

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
    }
}
