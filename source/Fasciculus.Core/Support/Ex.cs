using Fasciculus.IO.Resources;
using System;
using System.IO;

namespace Fasciculus.Support
{
    /// <summary>
    /// Exception helper.
    /// </summary>
    public static class Ex
    {
        private static string Format(string? format, params object[] args)
            => format is null ? string.Empty : string.Format(format, args);

        /// <summary>
        /// Creates a new <see cref="NotImplementedException"/>.
        /// </summary>
        public static NotImplementedException NotImplemented(string? message = null)
            => message is null ? new() : new(message);

        /// <summary>
        /// Creates a new <see cref="InvalidOperationException"/>.
        /// </summary>
        public static InvalidOperationException InvalidOperation(string? message = null)
            => message is null ? new() : new(message);

        /// <summary>
        /// Creates a new <see cref="ArgumentException"/>.
        /// </summary>
        public static ArgumentException Argument()
            => new();

        /// <summary>
        /// Creates a new <see cref="IndexOutOfRangeException"/>.
        /// </summary>
        public static IndexOutOfRangeException IndexOutOfRange()
            => new();

        /// <summary>
        /// Creates a new <see cref="FileNotFoundException"/>.
        /// </summary>
        public static FileNotFoundException FileNotFound(FileInfo? fileInfo = null)
            => new(Format(fileInfo?.FullName));

        /// <summary>
        /// Creates a new <see cref="EndOfStreamException"/>.
        /// </summary>
        public static EndOfStreamException EndOfStream()
            => new();

        /// <summary>
        /// Creates a new <see cref="ResourceNotFoundException"/>.
        /// </summary>
        public static ResourceNotFoundException ResourceNotFound(string? name = null)
            => new(Format(name));
    }
}
