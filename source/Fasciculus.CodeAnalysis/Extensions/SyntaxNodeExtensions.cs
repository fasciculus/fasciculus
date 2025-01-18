using Fasciculus.Net.Navigating;
using Microsoft.CodeAnalysis;
using System.IO;

namespace Fasciculus.CodeAnalysis.Extensions
{
    public static class SyntaxNodeExtensions
    {
        public static UriPath GetSource(this SyntaxNode node, DirectoryInfo projectDirectory)
            => node.SyntaxTree.GetSource(projectDirectory);
    }
}
