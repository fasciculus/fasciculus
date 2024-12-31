using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Linq;

namespace Fasciculus.GitHub
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using WebApplication app = CreateWebApplication();

            if (args.Contains("generate"))
            {
                Generator.Generate(app);
            }
            else
            {
                app.Run();
            }
        }

        private static WebApplication CreateWebApplication()
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder([]);

            builder.Services.AddControllersWithViews();

            LogServices(builder.Services);

            WebApplication app = builder.Build();

            app.UseRouting();

            app.MapStaticAssets();
            app.MapControllers();

            LogServices(app.Services);

            return app;
        }

        private static void LogServices(IServiceCollection services)
        {
            var typeNames = services.Select(s => s.ServiceType.FullName).Order();

            foreach (var typeName in typeNames)
            {
                Debug.WriteLine(typeName);
            }
        }

        private static void LogServices(IServiceProvider serviceProvider)
        {
            Debug.WriteLine(serviceProvider.GetType().FullName);
        }
    }
}
