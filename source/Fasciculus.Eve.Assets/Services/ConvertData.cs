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
        private readonly IDownloadSde downloadSde;
        private readonly IParseData parseData;
        private readonly IAssetsProgress progress;

        private EveData? result;
        private readonly TaskSafeMutex mutex = new();

        public ConvertData(IDownloadSde downloadSde, IParseData parseData, IAssetsProgress progress)
        {
            this.downloadSde = downloadSde;
            this.parseData = parseData;
            this.progress = progress;
        }

        public async Task<EveData> ConvertAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            await Task.Yield();

            if (result is null)
            {
                SdeData sdeData = await parseData.ParseAsync();

                progress.ConvertData.Report(PendingToDone.Working);

                DateTime version = downloadSde.Download().LastWriteTimeUtc;
                EveType.Data[] types = ConvertTypes(sdeData.Types);
                EveData.Data data = new(version, types);

                result = new(data);

                progress.ConvertData.Report(PendingToDone.Done);
            }

            return result;
        }

        private static EveType.Data[] ConvertTypes(Dictionary<long, SdeType> types)
        {
            return types
                .Select(ConvertType)
                .OrderBy(t => t.Id)
                .ToArray();
        }

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
            services.AddSdeZip();
            services.AddAssetsProgress();

            services.TryAddSingleton<IConvertData, ConvertData>();

            return services;
        }
    }
}
