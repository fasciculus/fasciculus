﻿using Fasciculus.Maui.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;

[assembly: XmlnsDefinition("http://fasciculus.github.io/2024/maui", "Fasciculus.Maui.Controls")]
[assembly: XmlnsPrefix("http://fasciculus.github.io/2024/maui", "fasciculus")]

namespace Fasciculus.Maui
{
    public static class FasciculusMauiConfiguration
    {
        public static MauiAppBuilder UseFasciculusMaui(this MauiAppBuilder builder)
        {
            IServiceCollection services = builder.Services;

            services.TryAddSingleton<IExceptions, Exceptions>();
            services.TryAddSingleton<INavigator, Navigator>();

            return builder;
        }
    }
}
