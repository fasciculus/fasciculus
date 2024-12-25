using Fasciculus.IO.Compressing;
using Fasciculus.Support;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fasciculus.IO
{
    public class ResourceNotFoundException : IOException
    {
        public ResourceNotFoundException() { }

        public ResourceNotFoundException(string name) : base(name) { }
    }

    public interface IEmbeddedResource
    {
        public T Read<T>(Func<Stream, T> read, bool compressed);
    }

    public class EmbeddedResource : IEmbeddedResource
    {
        private readonly Assembly assembly;
        private readonly string name;

        public EmbeddedResource(Assembly assembly, string name)
        {
            this.assembly = assembly;
            this.name = name;
        }

        public T Read<T>(Func<Stream, T> read, bool compressed)
        {
            using Stream? stream = assembly.GetManifestResourceStream(name);

            if (stream is null)
            {
                throw Ex.ResourceNotFound(name);
            }

            if (compressed)
            {
                using MemoryStream uncompressed = new();

                GZip.Extract(stream, uncompressed);
                uncompressed.Position = 0;
                return read(uncompressed);
            }
            else
            {
                return read(stream);
            }
        }
    }

    public interface IEmbeddedResources
    {
        public IEmbeddedResource this[string name] { get; }
    }

    public class EmbeddedResources : IEmbeddedResources
    {
        public IEmbeddedResource this[string name] => Find(name);

        private IEmbeddedResource Find(string name)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string[] names = assembly.GetManifestResourceNames();

                if (names.Contains(name))
                {
                    return new EmbeddedResource(assembly, name);
                }
            }

            throw Ex.ResourceNotFound(name);
        }
    }
}
