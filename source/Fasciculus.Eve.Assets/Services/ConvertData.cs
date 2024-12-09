using Fasciculus.Eve.Assets.Models;
using Fasciculus.Eve.Models;
using Fasciculus.Maui.Support;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IConvertData
    {
        public Task<EveData.Data> Data { get; }
    }

    public class ConvertData : IConvertData
    {
        private readonly IParseData parseData;
        private readonly IAssetsProgress progress;

        private EveData.Data? data;
        private readonly TaskSafeMutex dataMutex = new();

        public Task<EveData.Data> Data => GetDataAsync();

        public ConvertData(IParseData parseData, IAssetsProgress progress)
        {
            this.parseData = parseData;
            this.progress = progress;
        }

        private async Task<EveData.Data> GetDataAsync()
        {
            using Locker locker = Locker.Lock(dataMutex);

            if (data is null)
            {
                SdeData sdeData = await parseData.Data;

                progress.ConvertDataProgress.Report(WorkState.Working);

                DateTime version = sdeData.Version;
                IEnumerable<EveType.Data> types = ConvertTypes(sdeData.Types);
                IEnumerable<EveStationOperation.Data> stationOperations = ConvertStationOperations(sdeData.StationOperations);
                IEnumerable<EveNpcCorporation.Data> npcCorporations = ConvertNpcCorporations(sdeData.NpcCorporations);

                data = new(version, types, stationOperations, npcCorporations);

                progress.ConvertDataProgress.Report(WorkState.Done);
            }

            return data;
        }

        private static IEnumerable<EveType.Data> ConvertTypes(Dictionary<int, SdeType> types)
            => types.Select(ConvertType).OrderBy(t => t.Id);

        private static EveType.Data ConvertType(KeyValuePair<int, SdeType> kvp)
        {
            SdeType sdeType = kvp.Value;

            int id = kvp.Key;
            string name = sdeType.Name.En;
            double volume = sdeType.Volume;

            return new(id, name, volume);
        }

        private static IEnumerable<EveStationOperation.Data> ConvertStationOperations(Dictionary<int, SdeStationOperation> stationOperations)
            => stationOperations.Select(ConvertStationOperation).OrderBy(t => t.Id);

        private static EveStationOperation.Data ConvertStationOperation(KeyValuePair<int, SdeStationOperation> kvp)
        {
            SdeStationOperation sdeStationOperation = kvp.Value;

            int id = kvp.Key;
            string name = sdeStationOperation.OperationNameID.En;

            return new(id, name);
        }

        private static IEnumerable<EveNpcCorporation.Data> ConvertNpcCorporations(Dictionary<int, SdeNpcCorporation> npcCorporations)
            => npcCorporations.Select(ConvertNpcCorporation).OrderBy(t => t.Id);

        private static EveNpcCorporation.Data ConvertNpcCorporation(KeyValuePair<int, SdeNpcCorporation> kvp)
        {
            SdeNpcCorporation sdeNpcCorporation = kvp.Value;

            int id = kvp.Key;
            string name = sdeNpcCorporation.NameID.En;

            return new(id, name);
        }
    }

    public static class ConvertDataServices
    {
        public static IServiceCollection AddConvertData(this IServiceCollection services)
        {
            services.AddParseData();
            services.AddAssetsProgress();

            services.TryAddSingleton<IConvertData, ConvertData>();

            return services;
        }
    }
}
