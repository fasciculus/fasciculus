using Fasciculus.Eve.Assets.Models;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IParseData
    {
        public Task<SdeData> ParseAsync();
    }

    public class ParseData : IParseData
    {
        private readonly IDownloadSde downloadSde;
        private readonly IExtractSde extractSde;
        private readonly IYaml yaml;
        private readonly IAssetsProgress progress;

        public ParseData(IDownloadSde downloadSde, IExtractSde extractSde, IYaml yaml, IAssetsProgress progress)
        {
            this.downloadSde = downloadSde;
            this.extractSde = extractSde;
            this.yaml = yaml;
            this.progress = progress;
        }

        public async Task<SdeData> ParseAsync()
        {
            await Task.Yield();

            FileInfo downloadedFile = await downloadSde.DownloadedFile;
            DateTime version = downloadedFile.LastWriteTimeUtc;
            SdeFiles sdeFiles = await extractSde.Files;
            Task<Dictionary<long, string>> names = ParseNamesAsync(sdeFiles);
            Task<Dictionary<long, SdeType>> types = ParseTypesAsync(sdeFiles);

            Task.WaitAll([names, types]);

            return new()
            {
                Version = version,
                Names = names.Result,
                Types = types.Result,
            };
        }

        private async Task<Dictionary<long, string>> ParseNamesAsync(SdeFiles sdeFiles)
        {
            await Task.Yield();

            progress.ParseNames.Report(PendingToDone.Working);

            Dictionary<long, string> names = yaml
                .Deserialize<SdeName[]>(sdeFiles.NamesYaml)
                .ToDictionary(n => n.ItemID, n => n.ItemName);

            progress.ParseNames.Report(PendingToDone.Done);

            return names;
        }

        private async Task<Dictionary<long, SdeType>> ParseTypesAsync(SdeFiles sdeFiles)
        {
            await Task.Yield();

            progress.ParseTypes.Report(PendingToDone.Working);

            Dictionary<long, SdeType> types = yaml
                .Deserialize<Dictionary<long, SdeType>>(sdeFiles.TypesYaml);

            progress.ParseTypes.Report(PendingToDone.Done);

            return types;
        }
    }

    public static class ParseDataServices
    {
        public static IServiceCollection AddParseData(this IServiceCollection services)
        {
            services.AddAssetsDirectories();
            services.AddAssetsProgress();

            services.AddSdeZip();
            services.AddYaml();

            services.TryAddSingleton<IParseData, ParseData>();

            return services;
        }
    }
}
