using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve
{
    public static class Constants
    {
        public static DirectoryInfo ResourcesDirectory
            => Directories.BaseDirectory.Combine("..", "..", "..", "..", "Fasciculus.Eve", "Resources").Existing();
    }
}
