using System.IO;

namespace Fasciculus.Site
{
    public static class Locations
    {
        private static readonly FileInfo AssemblyFile
            = new(typeof(Locations).Assembly.Location);

        public static readonly DirectoryInfo ProjectDirectory
            = AssemblyFile.Directory!.Parent!.Parent!.Parent!;

        public static readonly DirectoryInfo OutputDirectory
            = new(Path.Combine(ProjectDirectory.Parent!.Parent!.Parent!.FullName, "fasciculus.github.io"));
    }
}
