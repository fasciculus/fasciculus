using Fasciculus.IO;
using System.IO;

namespace Fasciculus.Eve.Services
{
    public interface IEveFileSystem
    {
        public DirectoryInfo Documents { get; }
    }

    public class EveFileSystem : IEveFileSystem
    {
        public DirectoryInfo Documents { get; }

        public EveFileSystem(ISpecialDirectories specialDirectories)
        {
            Documents = specialDirectories.Documents.Combine("Fasciculus.Eve").CreateIfNotExists();
        }
    }
}
