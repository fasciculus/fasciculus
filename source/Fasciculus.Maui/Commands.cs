using Fasciculus.Threading;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Windows.Input;

namespace Fasciculus.Maui
{
    public class LongRunningCommand : ICommand
    {
        private readonly Action action;

        private bool canExecute = true;
        private readonly ReentrantTaskSafeMutex mutex = new();

        public event EventHandler? CanExecuteChanged;

        public LongRunningCommand(Action action)
        {
            this.action = action;
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

                Tasks.LongRunning(action).ContinueWith((t) => { SetCanExecute(true); });
            }
        }

        private void SetCanExecute(bool value)
        {
            using Locker locker = Locker.Lock(mutex);

            canExecute = value;

            MainThread.BeginInvokeOnMainThread(() => CanExecuteChanged?.Invoke(this, new EventArgs()));
        }
    }
}
