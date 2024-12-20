using CommunityToolkit.Maui;
using Fasciculus.Eve.PageModels;
using Fasciculus.Eve.ViewModels;
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
            builder.UseFasciculusMaui();
            builder.UseEveCore("Fasciculus.Eve (rhj1)");

            ConfigureFonts(builder);
            ConfigureLogging(builder.Logging);
            ConfigureServices(builder.Services);

            MauiApp app = builder.Build();

            ServiceRegistry.Initialize(app.Services);

            return app;
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<NavBarViewModel>();
            services.TryAddSingleton<StatusBarViewModel>();

            services.TryAddSingleton<LoadingPageModel>();
            services.TryAddSingleton<IndustryPageModel>();
            services.TryAddSingleton<InfoPageModel>();
            services.TryAddSingleton<TradesPageModel>();
            services.TryAddSingleton<MapPageModel>();
            services.TryAddSingleton<PlanetsPageModel>();
            services.TryAddSingleton<SkillsPageModel>();
            services.TryAddSingleton<DebugPageModel>();
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
