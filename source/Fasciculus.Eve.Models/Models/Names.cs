﻿using Fasciculus.IO;
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

        public static void Read(Data data)
        {
            names.Clear();

            int count = data.ReadInt();

            for (int i = 0; i < count; i++)
            {
                int id = data.ReadInt();
                string name = data.ReadString();

                Set(id, name);
            }
        }

        public static void Write(Data data)
        {
            data.WriteInt(names.Count);

            foreach (var entry in names)
            {
                data.WriteInt(entry.Key);
                data.WriteString(entry.Value);
            }
        }
    }
}
