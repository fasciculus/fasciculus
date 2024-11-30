using Fasciculus.Eve.PageModels;
using Fasciculus.Eve.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Fasciculus.Eve
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
            services.AddEveResources();
            services.AddEsi();

            services.TryAddSingleton<SideBarModel>();
            services.TryAddSingleton<IndustryPageModel>();
            services.TryAddSingleton<InfoPageModel>();
            services.TryAddSingleton<MarketPageModel>();
            services.TryAddSingleton<MapPageModel>();
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
