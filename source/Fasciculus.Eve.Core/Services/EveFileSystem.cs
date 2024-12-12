using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve.Services
{
    public interface IEveFileSystem
    {
        public DirectoryInfo Documents { get; }

        public DirectoryInfo EsiCache { get; }
    }

    public class EveFileSystem : IEveFileSystem
    {
        public DirectoryInfo Documents { get; }

        public DirectoryInfo EsiCache => Documents.Combine("EsiCache").CreateIfNotExists();

        public EveFileSystem(ISpecialDirectories specialDirectories)
        {
            Documents = specialDirectories.Documents.Combine("Fasciculus.Eve").CreateIfNotExists();
        }
    }
}
