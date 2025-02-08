using Fasciculus.IO;
using Fasciculus.IO.Searching;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Docs.Content.Services
{
    public class BlogProvider
    {
        public string[] GetKeys()
            => [.. GetEntries().OrderByDescending(kvp => kvp.Value.LastWriteTime).Select(kvp => kvp.Key)];

        public string GetEntry(string key)
            => GetEntries().TryGetValue(key, out FileInfo? file) ? file.ReadAllText() : string.Empty;

        private static Dictionary<string, FileInfo> GetEntries()
            => GetFiles().ToDictionary(GetKey);

        private static string GetKey(FileInfo file)
        {
            string name = file.NameWithoutExtension();
            string month = file.Directory!.Name[1..];
            string year = file.Directory!.Parent!.Name[1..];

            return $"{year}.{month}.{name}";
        }

        private static IEnumerable<FileInfo> GetFiles()
        {
            FileInfo location = new(typeof(BlogProvider).Assembly.Location);
            SearchPath searchPath = new([location.Directory!], true);

            return FileSearch.Search("*.md", searchPath);
        }
    }
}
