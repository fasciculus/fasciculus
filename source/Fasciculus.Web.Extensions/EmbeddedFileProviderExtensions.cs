using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Web.Extensions
{
    public static class EmbeddedFileProviderExtensions
    {
        public static IEnumerable<string> GetSubPaths(this EmbeddedFileProvider provider, IFileInfo fileInfo)
        {
            if (fileInfo.Exists)
            {
                string[] parts = fileInfo.Name.Split('.');

                for (int i = 0; i < parts.Length; ++i)
                {
                    string prefix = string.Join("/", parts.Take(i));
                    string suffix = string.Join(".", parts.Skip(i));
                    string subPath = string.IsNullOrEmpty(prefix) ? suffix : $"{prefix}/{suffix}";
                    IFileInfo candidate = provider.GetFileInfo(subPath);

                    if (candidate.Exists)
                    {
                        yield return subPath;
                    }
                }
            }
        }

        public static IEnumerable<string> GetSubPaths(this EmbeddedFileProvider provider)
            => provider.GetDirectoryContents("").SelectMany(provider.GetSubPaths);
    }
}
