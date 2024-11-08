using Fasciculus.IO;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fasciculus.Reflection
{
    public static class EmbeddedResources
    {
        public static Assembly? Find(string name)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string[] names = assembly.GetManifestResourceNames();

                if (names.Contains(name))
                {
                    return assembly;
                }
            }

            return null;
        }

        public static T Read<T>(string name, Func<Stream, T> read)
        {
            Assembly assembly = Find(name) ?? throw new FileNotFoundException(name);
            using Stream stream = assembly.GetManifestResourceStream(name);

            return read(stream);
        }

        public static T ReadCompressed<T>(string name, Func<Stream, T> read)
        {
            Assembly assembly = Find(name) ?? throw new FileNotFoundException(name);
            using Stream compressed = assembly.GetManifestResourceStream(name);
            using Stream uncompressed = new MemoryStream();

            GZip.Extract(compressed, uncompressed);
            uncompressed.Position = 0;

            return read(uncompressed);
        }
    }
}
