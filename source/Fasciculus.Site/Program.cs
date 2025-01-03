﻿using Fasciculus.GitHub.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.IO;

namespace Fasciculus.GitHub
{
    public static class Program
    {
        private static bool generate = false;
        private static DirectoryInfo outputDirectory = new("site");

        public static void Main(string[] args)
        {
            ParseArguments(args);

            using WebApplication app = CreateWebApplication();

            InitializeExpensiveServices(app.Services);

            if (generate)
            {
                app.Services.GetRequiredService<Generator>().Run();
            }
            else
            {
                app.Run();
            }
        }

        private static void InitializeExpensiveServices(IServiceProvider services)
        {
            _ = services.GetRequiredService<ApiProvider>();
        }

        private static WebApplication CreateWebApplication()
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder([]);

            builder.Services.AddControllersWithViews();
            builder.Services.TryAddSingleton<ApiProvider>();

            if (generate)
            {
                builder.Services.AddGenerator();
            }

            WebApplication app = builder.Build();

            app.UseRouting();
            app.MapStaticAssets();
            app.MapControllers();

            return app;
        }

        private static IServiceCollection AddGenerator(this IServiceCollection services)
        {
            services.TryAddKeyedSingleton(Writer.OutputDirectoryKey, outputDirectory);

            services.TryAddSingleton<Documents>();
            services.TryAddSingleton<Writer>();
            services.TryAddSingleton<Generator>();

            return services;
        }

        private static void ParseArguments(string[] args)
        {
            generate = args.Length > 0 && "generate" == args[0];

            if (generate && args.Length > 1)
            {
                outputDirectory = new(Path.GetFullPath(args[1]));
            }
        }
    }
}
