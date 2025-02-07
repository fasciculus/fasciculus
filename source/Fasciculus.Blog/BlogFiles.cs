using Fasciculus.IO.Searching;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.Blog
{
    public static class BlogFiles
    {
        public static IEnumerable<FileInfo> GetFiles()
        {
            FileInfo location = new(typeof(BlogFiles).Assembly.Location);
            SearchPath searchPath = new([location.Directory!], true);

            return FileSearch.Search("*.md", searchPath);
        }
    }
}
