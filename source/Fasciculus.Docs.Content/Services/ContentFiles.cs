using Fasciculus.IO;
using Fasciculus.IO.Searching;
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
            projectDirectory = FileSearch
                .Search("Fasciculus.Docs.Content.csproj", SearchPath.WorkingDirectoryAndParents)
                .First()
                .Directory!;
        }

        public IEnumerable<FileInfo> GetFiles(string directory)
        {
            SearchPath searchPath = new([projectDirectory.Combine(directory)], true);

            return FileSearch.Search("*.md", searchPath);
        }
    }
}
