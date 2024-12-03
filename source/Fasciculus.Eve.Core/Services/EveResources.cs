using Fasciculus.Eve.Models;
using Fasciculus.IO;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Services
{
    public interface IEveResources
    {
        public EveData Data { get; }
        public EveUniverse Universe { get; }
    }

    public class EveResources : IEveResources
    {
        private readonly IEmbeddedResources resources;

        private EveData? data = null;
        private TaskSafeMutex dataMutex = new();

        private EveUniverse? universe = null;
        private TaskSafeMutex universeMutex = new();

        public EveData Data => GetData();
        public EveUniverse Universe => GetUniverse();

        public EveResources(IEmbeddedResources resources)
        {
            this.resources = resources;
        }

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
