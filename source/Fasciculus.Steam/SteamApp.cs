using System.IO;

namespace Fasciculus.Steam
{
    public class SteamApp
    {
        public readonly int Id;
        public readonly string Name;
        public readonly bool Installed;
        public readonly DirectoryInfo? InstallationDirectory;

        public SteamApp(int id, string name, bool installed, DirectoryInfo? installationDirectory)
        {
            Id = id;
            Name = name;
            Installed = installed;
            InstallationDirectory = installationDirectory;
        }

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}
