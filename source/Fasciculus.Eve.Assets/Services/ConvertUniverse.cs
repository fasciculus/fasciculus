using Fasciculus.Eve.Assets.Models;
using Fasciculus.Eve.Models;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IConvertUniverse
    {
        public Task<EveRegion.Data[]> ConvertAsync();
    }

    public class ConvertUniverse : IConvertUniverse
    {
        private readonly IParseData parseData;
        private readonly IParseUniverse parseUniverse;
        private readonly IAssetsProgress progress;

        private EveRegion.Data[]? result = null;
        private readonly TaskSafeMutex mutex = new();

        public ConvertUniverse(IParseData parseData, IParseUniverse parseUniverse, IAssetsProgress progress)
        {
            this.parseData = parseData;
            this.parseUniverse = parseUniverse;
            this.progress = progress;
        }

        public async Task<EveRegion.Data[]> ConvertAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            await Task.Yield();

            if (result is null)
            {
                Task<SdeData> sdeData = parseData.ParseAsync();
                Task<SdeRegion[]> sdeRegions = parseUniverse.ParseAsync();

                Task.WaitAll([sdeData, sdeRegions]);

                progress.ConvertUniverse.Report(PendingToDone.Working);
                result = sdeRegions.Result.Select(r => ConvertRegion(r, sdeData.Result)).ToArray();
                progress.ConvertUniverse.Report(PendingToDone.Done);
            }

            return result;
        }

        private static EveRegion.Data ConvertRegion(SdeRegion sdeRegion, SdeData sdeData)
        {
            int id = sdeRegion.RegionID;
            string name = sdeData.Names[id];
            EveConstellation.Data[] constellations = sdeRegion.Constellations.Select(c => ConvertConstellation(c, sdeData)).ToArray();

            return new(id, name, constellations);
        }

        private static EveConstellation.Data ConvertConstellation(SdeConstellation sdeConstellation, SdeData sdeData)
        {
            int id = sdeConstellation.ConstellationID;
            string name = sdeData.Names[id];

            return new(id, name);
        }
    }

    public static class ConvertUniverseServices
    {
        public static IServiceCollection AddConvertUniverse(this IServiceCollection services)
        {
            services.AddAssetsProgress();
            services.AddParseData();
            services.AddParseUniverse();

            services.TryAddSingleton<IConvertUniverse, ConvertUniverse>();

            return services;
        }
    }
}
