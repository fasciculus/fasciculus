using Fasciculus.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.Hosting
{
    public static class DI
    {
        public static HostApplicationBuilder CreateEmptyBuilder()
        {
            HostApplicationBuilderSettings settings = new() { DisableDefaults = true };

            return Host.CreateEmptyApplicationBuilder(settings);
        }

        public static IServiceCollection AddSpecialPaths(this IServiceCollection services)
        {
            services.TryAdd(ServiceDescriptor.Singleton<ISpecialPaths, SpecialPaths>());

            return services;
        }

        public static HostApplicationBuilder UseSpecialPaths(this HostApplicationBuilder builder)
        {
            builder.Services.AddSpecialPaths();

            return builder;
        }
    }
}
