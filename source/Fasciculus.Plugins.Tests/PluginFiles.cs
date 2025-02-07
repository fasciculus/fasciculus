using Fasciculus.IO;
using Fasciculus.IO.Searching;
using System.IO;
using System.Linq;

namespace Fasciculus.Plugins.Tests
{
    public static class PluginFiles
    {
#if DEBUG
        public const bool IsDebug = true;
#else
        public const bool IsDebug = false;
#endif

        public static FileInfo GetTestee()
        {
            return DirectorySearch
                .Search("Fasciculus.Plugins.Testee", SearchPath.WorkingDirectoryAndParents)
                .First()
                .Combine("bin", IsDebug ? "Debug" : "Release", "net9.0")
                .File("Fasciculus.Plugins.Testee.dll");
        }
    }
}
