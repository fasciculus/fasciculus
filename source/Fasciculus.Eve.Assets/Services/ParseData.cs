using Fasciculus.Eve.Assets.Models;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IParseData
    {
        public Task<DateTime> Version { get; }
        public Task<Dictionary<long, string>> Names { get; }
        public Task<Dictionary<long, SdeType>> Types { get; }
        public Task<Dictionary<long, SdeNpcCorporation>> NpcCorporations { get; }

        public Task<SdeData> Data { get; }
    }

    public class ParseData : IParseData
    {
        private readonly IDownloadSde downloadSde;
        private readonly IExtractSde extractSde;
        private readonly IYaml yaml;
        private readonly IAssetsProgress progress;

        private DateTime? version = null;
        private readonly TaskSafeMutex versionMutex = new();

        private Dictionary<long, string>? names = null;
        private readonly TaskSafeMutex namesMutex = new();

        private Dictionary<long, SdeType>? types = null;
        private readonly TaskSafeMutex typesMutex = new();

        private Dictionary<long, SdeNpcCorporation>? npcCorporations = null;
        private readonly TaskSafeMutex npcCorporationsMutex = new();

        private SdeData? data = null;
        private readonly TaskSafeMutex dataMutex = new();

        public Task<DateTime> Version => GetVersionAsync();
        public Task<Dictionary<long, string>> Names => GetNamesAsync();
        public Task<Dictionary<long, SdeType>> Types => GetTypesAsync();
        public Task<Dictionary<long, SdeNpcCorporation>> NpcCorporations => GetNpcCorporationsAsync();

        public Task<SdeData> Data => GetDataAsync();

        public ParseData(IDownloadSde downloadSde, IExtractSde extractSde, IYaml yaml, IAssetsProgress progress)
        {
            this.downloadSde = downloadSde;
            this.extractSde = extractSde;
            this.yaml = yaml;
            this.progress = progress;
        }

        private async Task<DateTime> GetVersionAsync()
        {
            using Locker locker = Locker.Lock(versionMutex);

            if (!version.HasValue)
            {
                FileInfo downloadedFile = await downloadSde.DownloadedFile;

                version = downloadedFile.LastWriteTimeUtc;
            }

            return version.Value;
        }

        private Task<Dictionary<long, string>> GetNamesAsync()
        {
            SdeFiles sdeFiles = Tasks.Wait(extractSde.Files);

            return Tasks.LongRunning(() => GetNames(sdeFiles));
        }

        private Dictionary<long, string> GetNames(SdeFiles sdeFiles)
        {
            using Locker locker = Locker.Lock(namesMutex);

            if (names is null)
            {
                progress.ParseNames.Report(PendingToDone.Working);

                names = yaml
                    .Deserialize<SdeName[]>(sdeFiles.NamesYaml)
                    .ToDictionary(n => n.ItemID, n => n.ItemName);

                progress.ParseNames.Report(PendingToDone.Done);
            }

            return names;
        }

        private Task<Dictionary<long, SdeType>> GetTypesAsync()
        {
            SdeFiles sdeFiles = Tasks.Wait(extractSde.Files);

            return Tasks.LongRunning(() => GetTypes(sdeFiles));
        }

        private Dictionary<long, SdeType> GetTypes(SdeFiles sdeFiles)
        {
            using Locker locker = Locker.Lock(typesMutex);

            if (types is null)
            {
                progress.ParseTypes.Report(PendingToDone.Working);
                types = yaml.Deserialize<Dictionary<long, SdeType>>(sdeFiles.TypesYaml);
                progress.ParseTypes.Report(PendingToDone.Done);
            }

            return types;
        }

        private Task<Dictionary<long, SdeNpcCorporation>> GetNpcCorporationsAsync()
        {
            SdeFiles sdeFiles = Tasks.Wait(extractSde.Files);

            return Tasks.Start(() => GetNpcCorporations(sdeFiles));
        }

        private Dictionary<long, SdeNpcCorporation> GetNpcCorporations(SdeFiles sdeFiles)
        {
            using Locker locker = Locker.Lock(npcCorporationsMutex);

            if (npcCorporations is null)
            {
                progress.ParseNpcCorporations.Report(PendingToDone.Working);
                npcCorporations = yaml.Deserialize<Dictionary<long, SdeNpcCorporation>>(sdeFiles.NpcCorporationsYaml);
                progress.ParseNpcCorporations.Report(PendingToDone.Done);
            }

            return npcCorporations;
        }

        private Task<SdeData> GetDataAsync()
        {
            return Tasks.LongRunning(GetData);
        }

        private SdeData GetData()
        {
            using Locker locker = Locker.Lock(dataMutex);

            if (data is null)
            {
                Task.WaitAll([Version, Names, Types, NpcCorporations]);

                data = new()
                {
                    Version = Version.Result,
                    Names = Names.Result,
                    Types = Types.Result,
                    NpcCorporations = NpcCorporations.Result
                };
            }

            return data;
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
