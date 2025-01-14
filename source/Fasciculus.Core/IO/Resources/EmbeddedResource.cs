﻿using Fasciculus.IO.Compressing;
using Fasciculus.Support;
using System;
using System.IO;
using System.Reflection;

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
        /// <param name="assembly"></param>
        /// <param name="name"></param>
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
        /// <typeparam name="T"></typeparam>
        /// <returns>The return value of <paramref name="read"/></returns>
        public T Read<T>(Func<Stream, T> read, bool compressed)
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
    }
}
