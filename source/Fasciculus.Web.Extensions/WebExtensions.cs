using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Web.Extensions
{
    public static class WebExtensions
    {
        public static IServiceCollection AddRequestInvoker(this IServiceCollection services)
        {
            services.TryAddSingleton<RequestInvoker>();

            return services;
        }
    }
}
