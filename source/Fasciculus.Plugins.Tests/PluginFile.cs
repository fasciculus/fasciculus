using Fasciculus.IO;
using Fasciculus.IO.Searching;
using System.IO;
using System.Linq;

namespace Fasciculus.Plugins.Tests
{
    public static class PluginFile
    {
#if DEBUG
        public const string Configuration = "Debug";
#else
        public const string Configuration = "Release";
#endif

        public static FileInfo GetTestee()
        {
            return DirectorySearch
                .Search("Fasciculus.Plugins.Testee", SearchPath.WorkingDirectoryAndParents)
                .First()
                .Combine("bin", Configuration, "net9.0")
                .File("Fasciculus.Plugins.Testee.dll");
        }
    }
}
