using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Web.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder UseApplicationInvoker(this WebApplicationBuilder builder)
        {
            builder.Services.TryAddSingleton<RequestInvoker>();

            return builder;
        }
    }
}
