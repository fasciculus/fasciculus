using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Threading;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Fasciculus.Maui.Services
{
    public interface IExceptions : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public Exception? Last { get; }
        public ObservableCollection<Exception> All { get; }

        public void Add(Exception exception);
        public void Clear();
    }

    public partial class Exceptions : MainThreadObservable, IExceptions
    {
        [ObservableProperty]
        private Exception? last = null;

        public ObservableCollection<Exception> All { get; } = [];

        public void Add(Exception exception)
        {
            Tasks.Wait(MainThread.InvokeOnMainThreadAsync(() =>
            {
                All.Add(exception);
                Last = exception;
            }));
        }

        public void Clear()
        {
            Tasks.Wait(MainThread.InvokeOnMainThreadAsync(() =>
            {
                All.Clear();
                Last = null;
            }));
        }
    }
}
