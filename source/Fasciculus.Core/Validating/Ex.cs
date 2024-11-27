using System;
using System.IO;

namespace Fasciculus.Validating
{
    public class ResourceNotFoundException : IOException
    {
        public ResourceNotFoundException() { }

        public ResourceNotFoundException(string name) : base(name) { }
    }

    public static class Ex
    {
        public static NotImplementedException NotImplemented()
            => new();

        public static FileNotFoundException FileNotFound(FileInfo? fileInfo = null)
            => fileInfo is null ? new() : new(fileInfo.FullName);

        public static ResourceNotFoundException ResourceNotFound(string? name = null)
            => name is null ? new() : new(name);
    }
}
