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
        private readonly IParseUniverse parseUniverse;
        private readonly IAssetsProgress progress;

        private EveRegion.Data[]? result = null;
        private readonly TaskSafeMutex mutex = new();

        public ConvertUniverse(IParseUniverse parseUniverse, IAssetsProgress progress)
        {
            this.parseUniverse = parseUniverse;
            this.progress = progress;
        }

        public async Task<EveRegion.Data[]> ConvertAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            if (result is null)
            {
                SdeRegion[] sdeRegions = await parseUniverse.ParseAsync();

                progress.ConvertUniverse.Report(PendingToDone.Working);
                result = sdeRegions.Select(ConvertRegion).ToArray();
                progress.ConvertUniverse.Report(PendingToDone.Done);
            }

            return result;
        }

        private EveRegion.Data ConvertRegion(SdeRegion sdeRegion)
        {
            return new(sdeRegion.RegionID);
        }
    }

    public static class ConvertUniverseServices
    {
        public static IServiceCollection AddConvertUniverse(this IServiceCollection services)
        {
            services.AddAssetsProgress();
            services.AddParseUniverse();

            services.TryAddSingleton<IConvertUniverse, ConvertUniverse>();

            return services;
        }
    }
}
