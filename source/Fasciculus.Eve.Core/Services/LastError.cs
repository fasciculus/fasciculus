using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Maui.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Fasciculus.Eve.Services
{
    public interface ILastError
    {
        public Exception? Error { get; set; }
    }

    public partial class LastError : MainThreadObservable, ILastError
    {
        [ObservableProperty]
        private Exception? error;
    }

    public static class LastErrorServices
    {
        public static IServiceCollection AddLastError(this IServiceCollection services)
        {
            services.TryAddSingleton<ILastError, LastError>();

            return services;
        }
    }
}
