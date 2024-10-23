using Fasciculus.IO;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fasciculus.Reflection
{
    public static class EmbeddedResources
    {
        public static T Read<T>(string name, Func<Stream, T> read)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string[] names = assembly.GetManifestResourceNames();

                if (names.Contains(name))
                {
                    using Stream stream = assembly.GetManifestResourceStream(name);

                    return read(stream);
                }
            }

            throw new FileNotFoundException(name);
        }

        public static T Read<T>(string name, Func<Data, T> read)
        {
            return Read(name, (stream) => read(new Data(stream)));
        }
    }
}
