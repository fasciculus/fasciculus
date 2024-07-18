using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve
{
    public static class Constants
    {
        public static DirectoryInfo DataDirectory
            => Directories.Documents.Combine("Fasciculus", "Eve.Converter").Existing();

        public static DirectoryInfo BsdDirectory
            => DataDirectory.Combine("bsd");

        public static DirectoryInfo ResourcesDirectory
            => Directories.BaseDirectory.Combine("..", "..", "..", "..", "Fasciculus.Eve", "Resources").Existing();
    }
}
