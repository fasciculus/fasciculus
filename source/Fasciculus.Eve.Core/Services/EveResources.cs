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
    }

    public class EveResources : IEveResources
    {
        private readonly IEmbeddedResources resources;

        private EveData? data = null;
        private TaskSafeMutex dataMutex = new();

        public EveData Data => GetData();

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
