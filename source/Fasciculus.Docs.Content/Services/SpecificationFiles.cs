using Fasciculus.IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Docs.Content.Services
{
    public class SpecificationFiles
    {
        private readonly ContentFiles contentFiles;

        public SpecificationFiles(ContentFiles contentFiles)
        {
            this.contentFiles = contentFiles;
        }

        public IEnumerable<FileInfo> GetFiles()
            => contentFiles.GetFiles("Specifications");

        public IEnumerable<string> GetKeys()
            => GetFiles().OrderByDescending(f => f.LastWriteTime).Select(GetKey);

        public FileInfo GetFile(string key)
            => GetFiles().Where(f => GetKey(f) == key).First();

        private static string GetKey(FileInfo file)
        {
            string name = file.NameWithoutExtension();
            string package = file.Directory!.Name;

            return $"{package}.{name}";
        }
    }
}
