using Fasciculus.Docs.Content.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Docs.Content
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            IServiceCollection services = builder.Services;

            builder.Services.AddControllers();

            services.TryAddSingleton<VersionProvider>();
            services.TryAddSingleton<GraphicsProvider>();
            services.TryAddSingleton<BlogProvider>();

            WebApplication app = builder.Build();

            app.MapControllers();

            app.Services.GetRequiredService<BlogProvider>();

            app.Run();

        }
    }
}
