using Fasciculus.IO.Compressing;
using Fasciculus.Support;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Fasciculus.IO.Resources
{
    /// <summary>
    /// Represents an embedded resource.
    /// </summary>
    public class EmbeddedResource
    {
        /// <summary>
        /// The assembly in which the resource resides.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// The (logical) name of the resource.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes the embedded resource representation.
        /// </summary>
        public EmbeddedResource(Assembly assembly, string name)
        {
            Assembly = assembly;
            Name = name;
        }

        /// <summary>
        /// Read the resource represented by this information using the given <paramref name="read"/> function.
        /// <para>
        /// If <paramref name="compressed"/> is <c>true</c> the resource is expected to be GZip compressed.
        /// </para>
        /// </summary>
        /// <returns>The return value of <paramref name="read"/></returns>
        public T Read<T>(Func<Stream, T> read, bool compressed = false)
        {
            using Stream? stream = Assembly.GetManifestResourceStream(Name) ?? throw Ex.ResourceNotFound(Name);

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

        /// <summary>
        /// Reads the resource into a string.
        /// <para>
        /// If <paramref name="compressed"/> is <c>true</c> the resource is expected to be GZip compressed.
        /// </para>
        /// </summary>
        public string ReadString(Encoding encoding, bool compressed = false)
            => Read(s => s.ReadAllText(encoding), compressed);

        /// <summary>
        /// Reads the UTF-8 encoded resource into a string.
        /// <para>
        /// If <paramref name="compressed"/> is <c>true</c> the resource is expected to be GZip compressed.
        /// </para>
        /// </summary>
        public string ReadString(bool compressed = false)
            => ReadString(Encoding.UTF8, compressed);
    }
}
