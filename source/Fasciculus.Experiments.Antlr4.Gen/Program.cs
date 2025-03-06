using Fasciculus.IO;
using Fasciculus.NuGet;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System.IO;
using System.Linq;
using System.Threading;

namespace Fasciculus.Experiments.Antlr4.Gen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConsoleLogger logger = new();
            FileInfo configFile = FileSearch.Search("NuGet.config", SearchPath.WorkingDirectoryAndParents()).First();
            DirectoryInfo configDirectory = configFile.Directory!;
            ISettings settings = Settings.LoadDefaultSettings(configDirectory.FullName);
            SettingSection? section = settings.GetSection("config");
            AddItem? item = section?.GetFirstItemWithAttribute<AddItem>("key", "repositoryPath");
            string? repositoryPath = item?.Value;
            DirectoryInfo repositoryDirectory = configDirectory.Combine(repositoryPath!);
            PackageSource packageSource = new(repositoryDirectory.FullName);
            SourceRepository repository = Repository.Factory.GetCoreV3(packageSource);
            FindLocalPackagesResource findLocalPackages = repository.GetResource<FindLocalPackagesResource>();
            LocalPackageInfo[] packageInfos = [.. findLocalPackages.FindPackagesById("Antlr4.CodeGenerator", logger, CancellationToken.None)];
        }
    }
}
