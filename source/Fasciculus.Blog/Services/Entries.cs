using Fasciculus.IO;
using Fasciculus.IO.Searching;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Blog.Services
{
    public class Entries
    {
        private readonly Dictionary<string, FileInfo> files;

        public Entries(Graphics graphics)
        {
            files = GetFiles().ToDictionary(f => f.NameWithoutExtension());
        }

        public string[] GetKeys()
        {
            return [.. files.OrderByDescending(kvp => kvp.Value.LastWriteTime).Select(kvp => kvp.Key)];
        }

        public string GetEntry(string key)
        {
            return files[key].ReadAllText();
        }

        private static FileInfo[] GetFiles()
        {
            FileInfo location = new(typeof(Entries).Assembly.Location);
            SearchPath searchPath = new([location.Directory!], true);

            return [.. FileSearch.Search("*.md", searchPath)];
        }
    }
}
