using Fasciculus.Eve.Assets.Models;
using Fasciculus.Eve.Models;
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

        public Task<EveData.Data> Data => GetData();

        public ConvertData(IParseData parseData, IAssetsProgress progress)
        {
            this.parseData = parseData;
            this.progress = progress;
        }

        private async Task<EveData.Data> GetData()
        {
            using Locker locker = Locker.Lock(dataMutex);

            if (data is null)
            {
                SdeData sdeData = await parseData.Data;

                progress.ConvertData.Report(PendingToDone.Working);

                DateTime version = sdeData.Version;
                EveType.Data[] types = ConvertTypes(sdeData.Types);

                data = new(version, types);

                progress.ConvertData.Report(PendingToDone.Done);

                await Task.Yield();
            }

            return data;
        }

        private static EveType.Data[] ConvertTypes(Dictionary<long, SdeType> types)
            => [.. types.Select(ConvertType).OrderBy(t => t.Id)];

        private static EveType.Data ConvertType(KeyValuePair<long, SdeType> kvp)
        {
            SdeType sdeType = kvp.Value;

            long id = kvp.Key;
            string name = sdeType.Name.En;
            double volume = sdeType.Volume;

            return new(id, name, volume);
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
