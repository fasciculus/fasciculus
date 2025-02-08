using Fasciculus.Docs.Preview.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;

namespace Fasciculus.Docs.Preview
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using WebApplication app = CreateWebApplication(args);

            app.Run();
        }

        private static WebApplication CreateWebApplication(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            IServiceCollection services = builder.Services;

            services.AddControllersWithViews();

            services.TryAddSingleton<HttpClient>();
            services.TryAddSingleton<ContentClient>();
            services.TryAddSingleton<GraphicsClient>();
            services.TryAddSingleton<BlogClient>();
            services.TryAddSingleton<Renderer>();

            return CreateWebApplication(builder);
        }

        private static WebApplication CreateWebApplication(WebApplicationBuilder builder)
        {
            WebApplication app = builder.Build();

            app.UseRouting();
            app.MapStaticAssets();
            app.MapControllers();

            return app;
        }
    }
}
