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

            ConfigureFonts(builder);
            ConfigureLogging(builder.Logging);
            ConfigureServices(builder.Services);

            return builder.Build();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddAssetsProgress();
            services.AddSdeZip();
            services.AddParseData();

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
