using Fasciculus.IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Docs.Content.Services
{
    public class ContentFiles
    {
        private readonly DirectoryInfo projectDirectory;

        public ContentFiles()
        {
            projectDirectory = DirectorySearch
                .Search("source", SearchPath.WorkingDirectoryAndParents())
                .First().Combine("Fasciculus.Docs.Content");
        }

        public IEnumerable<FileInfo> GetFiles(string directory)
        {
            SearchPath searchPath = new([projectDirectory.Combine(directory)], true);

            return FileSearch.Search("*.md", searchPath);
        }
    }
}
