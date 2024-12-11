using CommunityToolkit.Maui;
using Fasciculus.Eve.PageModels;
using Fasciculus.Eve.Services;
using Fasciculus.Eve.ViewModels;
using Fasciculus.Maui;
using Fasciculus.Maui.Support;
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

            MauiApp mauiApp = builder.Build();

            mauiApp.InitializeServices();

            return mauiApp;
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddEsi();
            services.AddTrade();
            services.AddPlanetaryIndustry();

            services.TryAddKeyedSingleton("EsiUserAgent", "Fasciculus.Eve (rhj1)");

            services.TryAddSingleton<NavBarViewModel>();
            services.TryAddSingleton<StatusBarViewModel>();

            services.TryAddSingleton<SideBarModel>();
            services.TryAddSingleton<StatusBarModel>();

            services.TryAddSingleton<LoadingPageModel>();
            services.TryAddSingleton<IndustryPageModel>();
            services.TryAddSingleton<InfoPageModel>();
            services.TryAddSingleton<MarketPageModel>();
            services.TryAddSingleton<MapPageModel>();
            services.TryAddSingleton<PlanetsPageModel>();
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
