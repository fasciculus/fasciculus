using Fasciculus.IO;
using Fasciculus.IO.Searching;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Docs.Content.Services
{
    public class BlogContent
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
            FileInfo project = FileSearch.Search("Fasciculus.Docs.Content.csproj", SearchPath.WorkingDirectoryAndParents).First();
            DirectoryInfo directory = project.Directory!.Combine("Blog");
            SearchPath searchPath = new([directory], true);

            return FileSearch.Search("*.md", searchPath);
        }
    }
}
