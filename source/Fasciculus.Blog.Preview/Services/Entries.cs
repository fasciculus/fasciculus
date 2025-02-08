using Fasciculus.Blog.Preview.Models;
using Fasciculus.IO;
using Fasciculus.IO.Searching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Blog.Preview.Services
{
    public class Entries
    {
        private readonly FileInfo plugin;

        public DateTime Version => GetVersion();

        public Entries()
        {
            plugin = GetPluginFile();
        }

        public IEnumerable<string> GetKeys()
        {
            return GetFiles().OrderByDescending(f => f.LastWriteTimeUtc).Select(f => f.NameWithoutExtension());
        }

        public Entry GetEntry(string key)
        {
            return new()
            {
                Title = key,
                Content = key
            };
        }

        private DateTime GetVersion()
        {
            return DateTime.Now;
        }

        private FileInfo[] GetFiles()
        {
            return [];
        }

        private static FileInfo GetPluginFile()
        {
#if DEBUG
            string configuration = "Debug";
#else
            string configuration = "Release";
#endif

            return DirectorySearch
                .Search("Fasciculus.Blog", SearchPath.WorkingDirectoryAndParents)
                .First()
                .Combine("bin", configuration, "net9.0")
                .File("Fasciculus.Blog.dll");
        }
    }
}
