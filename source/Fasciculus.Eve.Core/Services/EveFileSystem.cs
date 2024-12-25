using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve.Services
{
    public interface IEveFileSystem
    {
        public DirectoryInfo Documents { get; }
        public FileInfo UserSettings { get; }
        public FileInfo SkillSettings { get; }

        public DirectoryInfo EsiCache { get; }
    }

    public class EveFileSystem : IEveFileSystem
    {
        public DirectoryInfo Documents { get; }

        public FileInfo UserSettings => Documents.File("Settings.json");
        public FileInfo SkillSettings => Documents.File("Skills.json");

        public DirectoryInfo EsiCache => Documents.Combine("EsiCache").CreateIfNotExists();

        public EveFileSystem()
        {
            Documents = SpecialDirectories.Documents.Combine("Fasciculus.Eve").CreateIfNotExists();
        }
    }
}
