using Fasciculus.IO;
using Fasciculus.NuGet.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VerifyMSTest;
using VerifyTests;

namespace Fasciculus.NuGet.Tests
{
    [TestClass]
    [UsesVerify]
    public partial class StaticRepositoryTests
    {
        private static DirectoryInfo Files { get; } = GetFilesDirectory();
        private static DirectoryInfo Generated => Files.Combine("Generated");
        private static DirectoryInfo Verified => Files.Combine("Verified");
        private static VerifySettings Settings { get; } = GetVerifySettings();

        [TestMethod]
        public async Task Test()
        {
            StaticRepository repository = new();

            repository.Write(Generated);

            string json;

            Settings.UseFileName("index.json");
            json = Generated.File("index.json").ReadAllText();
            await Verifier.VerifyJson(json, Settings);
        }

        private static DirectoryInfo GetFilesDirectory()
        {
            return FileSearch
                .Search("Fasciculus.NuGet.Tests.csproj", SearchPath.WorkingDirectoryAndParents())
                .First()
                .Directory!
                .Combine("StaticRepositoryTests");
        }

        private static VerifySettings GetVerifySettings()
        {
            VerifySettings settings = new();

            settings.UseDirectory(Verified.FullName);

            return settings;
        }
    }
}
