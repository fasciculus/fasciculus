using Fasciculus.Eve.Assets.Models;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IParseNames
    {
        public Dictionary<long, string> Parse();
    }

    public class ParseNames : IParseNames
    {
        private readonly IExtractSde extractSde;
        private readonly IAssetsFiles assetsFiles;
        private readonly IYaml yaml;
        private readonly IProgress<PendingOrDone> progress;

        private Dictionary<long, string>? result;
        private readonly TaskSafeMutex resultMutex = new();

        public ParseNames(IExtractSde extractSde, IAssetsFiles assetsFiles, IYaml yaml,
            [FromKeyedServices(ServiceKeys.ParseNames)] IProgress<PendingOrDone> progress)
        {
            this.extractSde = extractSde;
            this.assetsFiles = assetsFiles;
            this.yaml = yaml;
            this.progress = progress;
        }

        public Dictionary<long, string> Parse()
        {
            using Locker locker = Locker.Lock(resultMutex);

            if (result is null)
            {
                extractSde.Extract();

                progress.Report(PendingOrDone.Pending);

                NameSde[] names = yaml.Deserialize<NameSde[]>(assetsFiles.NamesYaml);

                result = names.ToDictionary(name => name.ItemID, name => name.ItemName);

                progress.Report(PendingOrDone.Done);
            }

            return result;
        }
    }

    public static class ParseNamesServices
    {
        public static IServiceCollection AddParseNames(this IServiceCollection services)
        {
            services.AddSdeZip();
            services.AddAssetsFileSystem();
            services.AddYaml();
            services.AddAssetsProgress();

            services.TryAddSingleton<IParseNames, ParseNames>();

            return services;
        }
    }
}
