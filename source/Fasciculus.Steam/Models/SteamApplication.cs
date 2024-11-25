using System.Diagnostics;
using System.IO;

namespace Fasciculus.Steam.Models
{
    [DebuggerDisplay("{Name} ({Id})")]
    public class SteamApplication
    {
        public int Id { get; }
        public string Name { get; }
        public readonly DirectoryInfo? InstallationDirectory;

        public bool IsInstalled => InstallationDirectory is not null;

        public SteamApplication(int id, string name, DirectoryInfo? installationDirectory)
        {
            Id = id;
            Name = name;
            InstallationDirectory = installationDirectory;
        }
    }
}
