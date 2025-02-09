using Fasciculus.IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Docs.Content.Services
{
    public class BlogFiles
    {
        private readonly ContentFiles contentFiles;

        public BlogFiles(ContentFiles contentFiles)
        {
            this.contentFiles = contentFiles;
        }

        public IEnumerable<FileInfo> GetFiles()
            => contentFiles.GetFiles("Blog");

        public IEnumerable<string> GetKeys()
            => GetFiles().OrderByDescending(f => f.LastWriteTime).Select(GetKey);

        public FileInfo GetFile(string key)
            => GetFiles().Where(f => GetKey(f) == key).First();

        private static string GetKey(FileInfo file)
        {
            string name = file.NameWithoutExtension();
            string month = file.Directory!.Name[1..];
            string year = file.Directory!.Parent!.Name[1..];

            return $"{year}.{month}.{name}";
        }
    }
}
