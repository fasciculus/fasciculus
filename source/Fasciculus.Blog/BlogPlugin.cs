using Fasciculus.IO.Searching;
using System;
using System.IO;

namespace Fasciculus.Blog
{
    public class BlogPlugin
    {
        public DateTime Version => GetVersion();

        private DateTime GetVersion()
        {
            FileInfo file = new(GetType().Assembly.Location);

            return file.LastWriteTimeUtc;
        }

        public FileInfo[] GetFiles()
        {
            FileInfo location = new(typeof(BlogPlugin).Assembly.Location);
            SearchPath searchPath = new([location.Directory!], true);

            return [.. FileSearch.Search("*.md", searchPath)];
        }
    }
}
