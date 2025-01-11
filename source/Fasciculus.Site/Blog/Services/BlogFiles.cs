using Fasciculus.IO;
using Fasciculus.IO.Searching;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Blog.Services
{
    public class BlogFiles : IEnumerable<FileInfo>
    {
        public DirectoryInfo Directory { get; }

        private readonly List<FileInfo> files;

        public IEnumerable<FileInfo> Files => files;

        public BlogFiles()
        {
            Directory = FindDirectory();
            files = new(Directory.GetFiles("*.md", SearchOption.AllDirectories));
        }

        private static DirectoryInfo FindDirectory()
        {
            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents;
            DirectoryInfo project = DirectorySearch.Search("Fasciculus.Site", searchPath).First();

            return project.Combine("Blog", "Documents");
        }

        public IEnumerator<FileInfo> GetEnumerator()
            => files.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => files.GetEnumerator();
    }
}
