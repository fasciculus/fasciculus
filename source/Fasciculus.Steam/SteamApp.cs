using System.IO;

namespace Fasciculus.Steam
{
    public class SteamApp
    {
        public int Id { get; }
        public string Name { get; }
        public readonly DirectoryInfo? InstallationDirectory;

        public bool IsInstalled => InstallationDirectory is not null;

        public SteamApp(int id, string name, DirectoryInfo? installationDirectory)
        {
            Id = id;
            Name = name;
            InstallationDirectory = installationDirectory;
        }

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}
