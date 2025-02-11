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

            services.TryAddSingleton<ContentVersion>();
            services.TryAddSingleton<ContentGraphics>();
            services.TryAddSingleton<ContentFiles>();
            services.TryAddSingleton<BlogFiles>();
            services.TryAddSingleton<SpecificationFiles>();
            services.TryAddSingleton<PackageFiles>();

            WebApplication app = builder.Build();

            app.MapControllers();

            app.Run();
        }
    }
}
