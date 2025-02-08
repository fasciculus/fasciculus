using Fasciculus.Blog.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Blog
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            IServiceCollection services = builder.Services;

            builder.Services.AddControllers();

            services.TryAddSingleton<Graphics>();
            services.TryAddSingleton<Entries>();

            WebApplication app = builder.Build();

            app.MapControllers();

            app.Run();
        }
    }
}
