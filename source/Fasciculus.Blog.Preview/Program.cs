using Fasciculus.Blog.Preview.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Blog.Preview
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using WebApplication app = CreateWebApplication();

            app.Run();
        }

        private static WebApplication CreateWebApplication()
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
            IServiceCollection services = builder.Services;

            services.AddControllersWithViews();

            services.TryAddSingleton<Entries>();

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
