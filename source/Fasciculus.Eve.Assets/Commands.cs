using Fasciculus.Eve.Assets.Services;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Windows.Input;

namespace Fasciculus.Eve.Assets
{
    public interface IStartCommand : ICommand
    {
    }

    public partial class StartCommand : IStartCommand
    {
        private readonly IExtractSde extractSde;

        private bool canExecute = true;
        private readonly ReentrantTaskSafeMutex mutex = new();

        public event EventHandler? CanExecuteChanged;

        public StartCommand(IExtractSde extractSde)
        {
            this.extractSde = extractSde;
        }

        public bool CanExecute(object? parameter)
        {
            using Locker locker = Locker.Lock(mutex);

            return canExecute;
        }

        public void Execute(object? parameter)
        {
            using Locker locker = Locker.Lock(mutex);

            if (canExecute)
            {
                SetCanExecute(false);

                Tasks.LongRunning(() => extractSde.Extract()).ContinueWith((t) => { SetCanExecute(true); });
            }
        }

        private void SetCanExecute(bool value)
        {
            using Locker locker = Locker.Lock(mutex);

            canExecute = value;

            MainThread.InvokeOnMainThreadAsync(() => CanExecuteChanged?.Invoke(this, new EventArgs()));
        }
    }

    public static class CommandServices
    {
        public static IServiceCollection AddCommands(this IServiceCollection services)
        {
            services.TryAddSingleton<IStartCommand, StartCommand>();

            return services;
        }
    }
}
