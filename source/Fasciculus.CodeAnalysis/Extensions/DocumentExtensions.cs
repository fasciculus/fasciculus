using Microsoft.CodeAnalysis;
using System.IO;

namespace Fasciculus.CodeAnalysis.Extensions
{
    public static class DocumentExtensions
    {
        public static bool IsGenerated(this Document document)
            => document.FilePath is not null && GeneratedCodeHelper.IsGenerated(new FileInfo(document.FilePath));
    }
}
