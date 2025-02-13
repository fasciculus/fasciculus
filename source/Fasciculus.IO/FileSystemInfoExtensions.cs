using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.IO
{
    /// <summary>
    /// Extensions for 
    /// </summary>
    public static class FileSystemInfoExtensions
    {
        /// <summary>
        /// Returns the relative path from the <paramref name="origin"/> to the <paramref name="target"/>
        /// prepending the optionally <paramref name="prefix"/>.
        /// </summary>
        public static string[] RelativeTo(this FileSystemInfo origin, FileSystemInfo target, IEnumerable<string>? prefix = null)
        {
            Uri originUri = new(origin.FullName);
            Uri targetUri = new(target.FullName);
            Uri relativeUri = targetUri.MakeRelativeUri(originUri);
            string[] parts = relativeUri.OriginalString.Split('/');
            IEnumerable<string> result = parts.Length > 0 ? parts.Skip(1) : [];

            prefix ??= [];

            return [.. prefix.Concat(result)];
        }
    }
}
