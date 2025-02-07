using Fasciculus.Blog.Preview.Models;
using Fasciculus.IO;
using System.Collections.Generic;
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
                .Select(f => f.NameWithoutExtension());
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
