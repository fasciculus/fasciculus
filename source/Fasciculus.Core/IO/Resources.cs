using Fasciculus.Validating;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fasciculus.IO
{
    public interface IEmbeddedResource
    {
        public T Read<T>(Func<Stream, T> read, bool compressed);
    }

    public class EmbeddedResource : IEmbeddedResource
    {
        private readonly ICompression compression;
        private readonly Assembly assembly;
        private readonly string name;

        public EmbeddedResource(ICompression compression, Assembly assembly, string name)
        {
            this.compression = compression;
            this.assembly = assembly;
            this.name = name;
        }

        public T Read<T>(Func<Stream, T> read, bool compressed)
        {
            using Stream stream = assembly.GetManifestResourceStream(name);

            if (compressed)
            {
                using MemoryStream uncompressed = new();

                compression.UnGZip(stream, uncompressed);
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
        private readonly ICompression compression;

        public IEmbeddedResource this[string name] => Find(name);

        public EmbeddedResources(ICompression compression)
        {
            this.compression = compression;
        }

        private IEmbeddedResource Find(string name)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string[] names = assembly.GetManifestResourceNames();

                if (names.Contains(name))
                {
                    return new EmbeddedResource(compression, assembly, name);
                }
            }

            throw Ex.ResourceNotFound(name);
        }
    }

    public static class ResourceServices
    {
        public static IServiceCollection AddResources(this IServiceCollection services)
        {
            services.AddCompression();

            services.TryAddSingleton<IEmbeddedResources, EmbeddedResources>();

            return services;
        }
    }
}
