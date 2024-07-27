using Fasciculus.IO;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fasciculus.Reflection
{
    public static class EmbeddedResources
    {
        public static void Read(string name, Action<Stream> read)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string[] names = assembly.GetManifestResourceNames();

                if (names.Contains(name))
                {
                    using Stream stream = assembly.GetManifestResourceStream(name);

                    read(stream);

                    return;
                }
            }

            throw new FileNotFoundException(name);
        }

        public static void Read(string name, Action<Data> read)
        {
            Read(name, (stream) => read(new Data(stream)));
        }
    }
}
