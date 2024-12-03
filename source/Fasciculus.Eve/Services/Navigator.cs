using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Services
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
