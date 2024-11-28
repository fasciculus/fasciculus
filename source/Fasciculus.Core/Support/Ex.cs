using Fasciculus.IO;
using System;
using System.IO;

namespace Fasciculus.Support
{
    public static class Ex
    {
        private static string Format(string? format, params object[] args)
            => format is null ? string.Empty : string.Format(format, args);

        public static NotImplementedException NotImplemented()
            => new();

        public static FileNotFoundException FileNotFound(FileInfo? fileInfo = null)
            => new(Format(fileInfo?.FullName));

        public static ResourceNotFoundException ResourceNotFound(string? name = null)
            => new(Format(name));
    }
}
