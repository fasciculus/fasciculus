using Fasciculus.IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Docs.Content.Services
{
    public class PackageFiles
    {
        private readonly ContentFiles contentFiles;

        public PackageFiles(ContentFiles contentFiles)
        {
            this.contentFiles = contentFiles;
        }

        public IEnumerable<FileInfo> GetFiles()
            => contentFiles.GetFiles("Packages");

        public IEnumerable<string> GetKeys()
            => GetFiles().OrderByDescending(f => f.LastWriteTime).Select(GetKey);

        public bool Contains(string key)
            => GetKeys().Contains(key);

        public FileInfo GetFile(string key)
            => GetFiles().Where(f => GetKey(f) == key).First();

        private static string GetKey(FileInfo file)
            => file.NameWithoutExtension();
    }
}
