using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IResourcesCreator
    {
        public void Create();
    }

    public class ResourcesCreator : IResourcesCreator
    {
        private readonly IDataParser dataParser;

        public ResourcesCreator(IDataParser dataParser)
        {
            this.dataParser = dataParser;
        }

        public void Create()
        {
            dataParser.Parse();
        }
    }

    public static class ResourcesServices
    {
        public static IServiceCollection AddResourcesCreator(this IServiceCollection services)
        {
            services.AddDataParsers();

            services.TryAddSingleton<IResourcesCreator, ResourcesCreator>();

            return services;
        }
    }
}
