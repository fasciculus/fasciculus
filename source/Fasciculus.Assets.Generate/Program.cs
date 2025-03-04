using Fasciculus.IO;
using Fasciculus.IO.Searching;
using System.IO;
using System.Linq;

namespace Fasciculus.Assets
{
    public static class Program
    {
        public static void Main()
        {
            DirectoryInfo directory = TargetDirectory;
            FileInfo file;
            byte[] data;

            file = directory.File("fasciculus.png");
            data = CreateLogo.Create();

            file.WriteIfDifferent(data);

            file = directory.File("todo.png");
            data = CreateToDo.Create();

            file.WriteIfDifferent(data);

            file = directory.File("done.png");
            data = CreateDone.Create();

            file.WriteIfDifferent(data);
        }

        private static DirectoryInfo TargetDirectory { get; }
            = DirectorySearch.Search("Fasciculus.Assets.Shared", SearchPath.WorkingDirectoryAndParents()).First();
    }
}
