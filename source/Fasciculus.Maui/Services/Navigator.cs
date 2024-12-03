using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace Fasciculus.Maui.Services
{
    public interface INavigator
    {
        public Task GoTo(string url);
    }

    public class Navigator : INavigator
    {
        public Task GoTo(string url)
        {
            return MainThread.InvokeOnMainThreadAsync(() => InternalGoTo(url));
        }

        private Task InternalGoTo(string url)
        {
            return Shell.Current.GoToAsync(url)
                .ContinueWith(_ => Tasks.Wait(Task.Delay(250)));
        }
    }

    public static class NavigatorServices
    {
        public static IServiceCollection AddNavigator(this IServiceCollection services)
        {
            services.TryAddSingleton<INavigator, Navigator>();

            return services;
        }
    }
}
