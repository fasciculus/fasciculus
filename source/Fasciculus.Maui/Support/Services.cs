using Fasciculus.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using System;

namespace Fasciculus.Maui.Support
{
    public static class MauiAppServices
    {
        private static IServiceProvider? services;

        public static MauiApp InitializeServices(this MauiApp mauiApp)
        {
            services = mauiApp.Services;

            return mauiApp;
        }

        public static T GetRequiredService<T>()
            where T : notnull

        {
            return Cond.NotNull(services).GetRequiredService<T>();
        }
    }
}
