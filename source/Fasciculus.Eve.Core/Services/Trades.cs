using Fasciculus.Maui.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel;

namespace Fasciculus.Eve.Services
{
    public interface ITrades : INotifyPropertyChanged
    {

    }

    public partial class Trades : MainThreadObservable, ITrades
    {

    }

    public static class TradesServices
    {
        public static IServiceCollection AddTrades(this IServiceCollection services)
        {
            services.TryAddSingleton<ITrades, Trades>();

            return services;
        }
    }
}
