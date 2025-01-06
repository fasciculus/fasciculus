using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Support
{
    /// <summary>
    /// Utility class to determine whether a code file is generated.
    /// </summary>
    public static class DetectGeneratedCode
    {
        private static readonly string[] GeneratedFileNameEndings
            = [".designer", ".generated", ".g", ".g.i", "AssemblyInfo", "AssemblyAttributes"];

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

        /// <summary>
        /// Determines whether the given <paramref name="document"/> is generated.
        /// </summary>
        public static bool IsGenerated(this Document document)
            => document.FilePath is not null && IsGenerated(new FileInfo(document.FilePath));

        /// <summary>
        /// Determines whether the given <paramref name="syntaxTree"/> is generated.
        /// </summary>
        public static bool IsGenerated(this SyntaxTree syntaxTree)
            => !string.IsNullOrEmpty(syntaxTree.FilePath) && IsGenerated(new FileInfo(syntaxTree.FilePath));
    }
}
