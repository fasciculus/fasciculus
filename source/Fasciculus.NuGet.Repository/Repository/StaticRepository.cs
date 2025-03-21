using Fasciculus.IO;
using Fasciculus.NuGet.Repository.Models;
using System.IO;
using System.Text.Json;

namespace Fasciculus.NuGet.Repository
{
    public class StaticRepository
    {
        public void Write(DirectoryInfo directory)
        {
            WriteIndex(directory);
        }

        private void WriteIndex(DirectoryInfo directory)
        {
            RepositoryIndex index = new();
            FileInfo file = directory.File("index.json");
            string json = JsonSerializer.Serialize(index);

            file.WriteAllText(json);
        }
    }
}
