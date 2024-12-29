using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Fasciculus.FileServer
{
    public class Program
    {
        private static readonly FileServerOptions fileServerOptions = new()
        {
            EnableDirectoryBrowsing = true,
            StaticFileOptions =
            {
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/octet-stream"
            }
        };

        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDirectoryBrowser();

            WebApplication app = builder.Build();

            app.Use(TryHtml);
            app.UseFileServer(fileServerOptions);
            app.Run();
        }

        private static async Task TryHtml(HttpContext context, Func<Task> next)
        {
            await next.Invoke();

            if (context.Response.StatusCode != 404)
            {
                return;
            }

            context.Request.Path += ".html";

            await next.Invoke();
        }
    }
}
