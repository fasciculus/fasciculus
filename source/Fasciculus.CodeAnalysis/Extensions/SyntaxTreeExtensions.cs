using Fasciculus.IO;
using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Extensions
{
    public static class SyntaxTreeExtensions
    {
        public static FileInfo GetFile(this SyntaxTree syntaxTree)
            => new(syntaxTree.FilePath);

        public static UriPath GetSource(this SyntaxTree syntaxTree, DirectoryInfo projectDirectory)
            => new(syntaxTree.GetFile().RelativeTo(projectDirectory));

        /// <summary>
        /// Determines whether the given <paramref name="syntaxTree"/> is generated.
        /// </summary>
        public static bool IsGenerated(this SyntaxTree syntaxTree)
            => !string.IsNullOrEmpty(syntaxTree.FilePath) && GeneratedCodeHelper.IsGenerated(syntaxTree.GetFile());

        public static bool IsGenerated(this SyntaxTree syntaxTree, DirectoryInfo projectDirectory)
        {
            if (IsGenerated(syntaxTree))
            {
                return true;
            }

            UriPath source = syntaxTree.GetSource(projectDirectory);

            return source.Contains("obj") || source.Contains("bin");
        }
    }
}
