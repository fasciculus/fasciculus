using CommunityToolkit.Maui;
using Fasciculus.Eve.PageModels;
using Fasciculus.Eve.Services;
using Fasciculus.Maui;
using Fasciculus.Maui.Services;
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
            builder.UseMauiCommunityToolkit();
            builder.UseMauiFasciculus();

            ConfigureFonts(builder);
            ConfigureLogging(builder.Logging);
            ConfigureServices(builder.Services);

            return builder.Build();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddNavigator();
            services.AddLastError();
            services.AddEsi();
            services.AddTrade();
            services.AddPlanetaryIndustry();

            services.TryAddKeyedSingleton("EsiUserAgent", "Fasciculus.Eve (rhj1)");

            services.TryAddSingleton<SideBarModel>();
            services.TryAddSingleton<StatusBarModel>();

            services.TryAddSingleton<LoadingPageModel>();
            services.TryAddSingleton<IndustryPageModel>();
            services.TryAddSingleton<InfoPageModel>();
            services.TryAddSingleton<MarketPageModel>();
            services.TryAddSingleton<MapPageModel>();
            services.TryAddSingleton<PlanetaryIndustryPageModel>();
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
