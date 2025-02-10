using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Web.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder UseApplicationDelegate(this WebApplicationBuilder builder)
        {
            builder.Services.TryAddSingleton<WebApplicationDelegate>();

            return builder;
        }
    }
}
