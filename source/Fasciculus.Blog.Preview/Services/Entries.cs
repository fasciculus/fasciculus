using Fasciculus.Blog.Preview.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Blog.Preview.Services
{
    public class Entries
    {
        public IEnumerable<string> GetKeys()
        {
            return BlogFiles
                .GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .Select(f => Path.GetFileNameWithoutExtension(f.Name));
        }

        public Entry GetEntry(string key)
        {
            return new()
            {
                Title = key,
                Content = key
            };
        }
    }
}
