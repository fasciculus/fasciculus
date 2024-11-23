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
            ConfigureLogging(builder);

            return builder.Build();
        }

        private static void ConfigureFonts(MauiAppBuilder builder)
        {
            builder.ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        }

        private static void ConfigureLogging(MauiAppBuilder builder)
        {
#if DEBUG
            builder.Logging.AddDebug();
#endif
        }
    }
}
