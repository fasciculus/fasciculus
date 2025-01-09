using System;
using System.IO;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Extensions
{
    public static class GeneratedCodeHelper
    {
        private static readonly string[] GeneratedFileNameEndings
            = [".designer", ".generated", ".g", ".g.i"];

        /// <summary>
        /// Determines whether the given <paramref name="file"/> is generated.
        /// </summary>
        public static bool IsGenerated(FileInfo file)
        {
            string name = file.Name;

            if (name.StartsWith("TemporaryGeneratedFile_", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string extension = file.Extension;

            name = name[..^extension.Length];

            return GeneratedFileNameEndings.Any(s => name.EndsWith(s, StringComparison.OrdinalIgnoreCase));
        }
    }
}
