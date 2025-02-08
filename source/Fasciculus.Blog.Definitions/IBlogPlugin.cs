using System;
using System.IO;

namespace Fasciculus.Blog
{
    public interface IBlogPlugin
    {
        public DateTime Version { get; }

        public FileInfo[] GetFiles();
    }
}
