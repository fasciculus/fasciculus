using Fasciculus.Eve.Assets.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Fasciculus.Eve.Assets
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseMauiApp<App>();
            builder.UseFasciculusMaui();

            ConfigureFonts(builder);
            ConfigureLogging(builder.Logging);
            ConfigureServices(builder.Services);

            return builder.Build();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSteam();

            services.TryAddSingleton<IAssetsProgress, AssetsProgress>();
            services.TryAddSingleton<IAssetsDirectories, AssetsDirectories>();
            services.TryAddSingleton<IWriteResource, WriteResource>();

            services.TryAddSingleton<IDownloadSde, DownloadSde>();
            services.TryAddSingleton<IExtractSde, ExtractSde>();

            services.TryAddSingleton<IYaml, Yaml>();
            services.TryAddSingleton<IParseData, ParseData>();
            services.TryAddSingleton<IParseUniverse, ParseUniverse>();

            services.TryAddSingleton<IConvertData, ConvertData>();
            services.TryAddSingleton<IConvertUniverse, ConvertUniverse>();
            services.TryAddSingleton<ICreateNavigation, CreateNavigation>();

            services.TryAddSingleton<ICopyImages, CopyImages>();
            services.TryAddSingleton<ICreateImages, CreateImages>();

            services.TryAddSingleton<ICreateResources, CreateResources>();

            services.TryAddSingleton<MainPageModel>();
        }

        private static void ConfigureFonts(MauiAppBuilder builder)
        {
            builder.ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        }

        private static void ConfigureLogging(ILoggingBuilder builder)
        {
#if DEBUG
            builder.AddDebug();
#endif
        }
    }
}
