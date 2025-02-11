using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Docs.Content.Services
{
    public abstract class Files
    {
        private readonly ContentFiles contentFiles;
        private readonly string directory;

        protected Files(ContentFiles contentFiles, string directory)
        {
            this.contentFiles = contentFiles;
            this.directory = directory;
        }

        public IEnumerable<FileInfo> GetFiles()
            => contentFiles.GetFiles(directory);

        public IEnumerable<string> GetKeys()
            => GetFiles().OrderByDescending(f => f.LastWriteTime).Select(GetKey);

        public bool Contains(string key)
            => GetKeys().Contains(key);

        public FileInfo GetFile(string key)
            => GetFiles().Where(f => GetKey(f) == key).First();

        protected abstract string GetKey(FileInfo file);
    }
}
