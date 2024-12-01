using Fasciculus.Eve.Assets.Models;
using Fasciculus.Eve.Models;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IConvertData
    {
        public Task<EveData> ConvertAsync();
    }

    public class ConvertData : IConvertData
    {
        private readonly IParseData parseData;
        private readonly IAssetsProgress progress;

        private EveData? result;
        private readonly TaskSafeMutex mutex = new();

        public ConvertData(IParseData parseData, IAssetsProgress progress)
        {
            this.parseData = parseData;
            this.progress = progress;
        }

        public async Task<EveData> ConvertAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            if (result is null)
            {
                progress.ConvertData.Report(PendingToDone.Working);

                SdeData sdeData = await parseData.Data;
                DateTime version = sdeData.Version;
                EveType.Data[] types = ConvertTypes(sdeData.Types);
                EveData.Data data = new(version, types);

                result = new(data);

                progress.ConvertData.Report(PendingToDone.Done);

                await Task.Yield();
            }

            return result;
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
