using Fasciculus.Eve.Models;
using Fasciculus.IO;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Services
{
    public interface IEveResources
    {
        public Task<EveData> Data { get; }
        public Task<EveUniverse> Universe { get; }
        public Task<EveNavigation> Navigation { get; }
    }

    public class EveResources : IEveResources
    {
        private readonly IEmbeddedResources resources;

        private EveData? data = null;
        private readonly TaskSafeMutex dataMutex = new();

        private EveUniverse? universe = null;
        private readonly TaskSafeMutex universeMutex = new();

        private EveNavigation? navigation = null;
        private readonly TaskSafeMutex navigationMutex = new();

        public Task<EveData> Data => GetDataAsync();
        public Task<EveUniverse> Universe => GetUniverseAsync();
        public Task<EveNavigation> Navigation => GetNavigationAsync();

        public EveResources(IEmbeddedResources resources)
        {
            this.resources = resources;
        }

        private Task<EveData> GetDataAsync()
            => Tasks.Start(GetData);

        private EveData GetData()
        {
            using Locker locker = Locker.Lock(dataMutex);

            if (data is null)
            {
                IEmbeddedResource resource = resources["EveData"];

                data = resource.Read(s => new EveData(s), true);
            }

            return data;
        }

        private Task<EveUniverse> GetUniverseAsync()
            => Tasks.LongRunning(GetUniverse);

        private EveUniverse GetUniverse()
        {
            using Locker locker = Locker.Lock(universeMutex);

            if (universe is null)
            {
                IEmbeddedResource resource = resources["EveUniverse"];

                universe = resource.Read(s => new EveUniverse(s), true);
            }

            return universe;
        }

        private Task<EveNavigation> GetNavigationAsync()
            => Tasks.Start(GetNavigation);

        private EveNavigation GetNavigation()
        {
            using Locker locker = Locker.Lock(navigationMutex);

            if (navigation is null)
            {
                IEmbeddedResource resource = resources["EveNavigation"];

                navigation = resource.Read(s => new EveNavigation(s), true);
            }

            return navigation;
        }
    }

    public static class EveResourcesServices
    {
        public static IServiceCollection AddEveResources(this IServiceCollection services)
        {
            services.AddEmbeddedResources();

            services.TryAddSingleton<IEveResources, EveResources>();

            return services;
        }
    }
}
