using Microsoft.CodeAnalysis;
using System.IO;

namespace Fasciculus.CodeAnalysis.Extensions
{
    public static class SyntaxTreeExtensions
    {
        /// <summary>
        /// Determines whether the given <paramref name="syntaxTree"/> is generated.
        /// </summary>
        public static bool IsGenerated(this SyntaxTree syntaxTree)
            => !string.IsNullOrEmpty(syntaxTree.FilePath) && GeneratedCodeHelper.IsGenerated(new FileInfo(syntaxTree.FilePath));
    }
}
