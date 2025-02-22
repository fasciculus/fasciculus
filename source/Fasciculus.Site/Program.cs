using Fasciculus.Docs.Content.Services;
using Fasciculus.Markdown.Svg;
using Fasciculus.NuGet;
using Fasciculus.NuGet.Services;
using Fasciculus.Site.Api.Services;
using Fasciculus.Site.Blog.Compilers;
using Fasciculus.Site.Blog.Services;
using Fasciculus.Site.Generating.Services;
using Fasciculus.Site.Licenses.Services;
using Fasciculus.Site.Markdown;
using Fasciculus.Site.Releases.Services;
using Fasciculus.Site.Specifications.Services;
using Fasciculus.Web.Extensions;
using Fasciculus.Web.Resources;
using Fasciculus.Yaml;
using Markdig;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using System;
using System.IO;
using YamlDotNet.Serialization;

namespace Fasciculus.Site
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
                app.Services.GetRequiredService<GeneratorDeleter>().Run();
            }
            else
            {
                app.Run();
            }
        }

        private static void InitializeExpensiveServices(IServiceProvider services)
        {
            //_ = services.GetRequiredService<ApiContent>();
        }

        private static WebApplication CreateWebApplication()
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
            IServiceCollection services = builder.Services;

            builder.Services.AddControllersWithViews();

            services
                .AddCommon()
                .AddApi()
                .AddBlog()
                .AddSpecifications()
                .AddReleases()
                .AddLicenses();

            if (generate)
            {
                services.AddRequestInvoker();
                services.AddGenerator();
            }

            return CreateApplication(builder);
        }

        private static WebApplication CreateApplication(WebApplicationBuilder builder)
        {
            WebApplication app = builder.Build();

            app.UseBootstrapResources();
            app.UseKatexResources();
            app.UseSiteResources();

            app.UseRouting();
            app.MapStaticAssets();
            app.MapControllers();

            return app;
        }

        private static IServiceCollection AddCommon(this IServiceCollection services)
        {
            services.TryAddSingleton<IDeserializer>(YDeserializer.Instance);
            services.TryAddSingleton<ContentGraphics>();
            services.TryAddSingleton<ISvgMappings, SiteGraphics>();
            services.TryAddSingleton<MarkdownPipelineBuilder, PipelineBuilder>();
            services.TryAddSingleton(s => s.GetRequiredService<MarkdownPipelineBuilder>().Build());

            services.TryAddSingleton<ContentFiles>();

            return services;
        }

        private static IServiceCollection AddApi(this IServiceCollection services)
        {
            services.TryAddSingleton<PackageFiles>();
            services.TryAddSingleton<ApiPackages>();

            services.TryAddSingleton<NamespaceFiles>();
            services.TryAddSingleton<ApiNamespaces>();

            services.TryAddSingleton<ApiContent>();
            services.TryAddSingleton<ApiNavigation>();

            return services;
        }

        private static IServiceCollection AddBlog(this IServiceCollection services)
        {
            services.TryAddSingleton<BlogFiles>();
            services.TryAddSingleton<BlogCompiler>();
            services.TryAddSingleton<BlogContent>();
            services.TryAddSingleton<BlogNavigation>();

            return services;
        }

        private static IServiceCollection AddSpecifications(this IServiceCollection services)
        {
            services.TryAddSingleton<SpecificationFiles>();
            services.TryAddSingleton<SpecificationCompiler>();
            services.TryAddSingleton<SpecificationContent>();
            services.TryAddSingleton<SpecificationNavigation>();

            return services;
        }

        private static IServiceCollection AddReleases(this IServiceCollection services)
        {
            services.TryAddSingleton<RoadmapFiles>();
            services.TryAddSingleton<RoadmapCompiler>();
            services.TryAddSingleton<ReleasesCompiler>();
            services.TryAddSingleton<ReleasesContent>();
            services.TryAddSingleton<ReleasesNavigation>();

            return services;
        }

        private static IServiceCollection AddLicenses(this IServiceCollection services)
        {
            ConsoleLogger logger = new(LogLevel.Warning);

            services.TryAddSingleton<IProjectPackagesProvider, ProjectPackagesProvider>();
            services.TryAddSingleton<IDirectoryPackagesProvider, DirectoryPackagesProvider>();
            services.TryAddSingleton<INuGetResources, NuGetResources>();
            services.TryAddSingleton<SourceCacheContext>();
            services.TryAddSingleton<ILogger>(logger);
            services.TryAddSingleton<IMetadataProvider, MetadataProvider>();
            services.TryAddSingleton<IIgnoredPackages, IgnoredPackages>();
            services.TryAddSingleton<IDependencyProvider, DependencyProvider>();

            services.TryAddSingleton<LicensesContent>();

            return services;
        }

        private static IServiceCollection AddGenerator(this IServiceCollection services)
        {
            services.TryAddKeyedSingleton(GeneratorWriter.OutputDirectoryKey, outputDirectory);

            services.TryAddSingleton<GeneratorDocuments>();
            services.TryAddSingleton<GeneratorWriter>();
            services.TryAddSingleton<Generator>();
            services.TryAddSingleton<GeneratorDeleter>();

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
