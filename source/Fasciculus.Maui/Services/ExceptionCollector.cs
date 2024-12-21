using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Maui.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Fasciculus.Maui.Services
{
    public interface IExceptionCollector : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public Exception? Last { get; }
        public IReadOnlyList<Exception> Exceptions { get; }

        public void Add(Exception exception);
        public void Clear();
    }

    public partial class ExceptionCollector : ObservableObject, IExceptionCollector
    {
        [ObservableProperty]
        public partial Exception? Last { get; private set; }

        private readonly ObservableCollection<Exception> exceptions = [];

        public IReadOnlyList<Exception> Exceptions => exceptions;

        public ExceptionCollector()
        {
            exceptions.CollectionChanged += OnExceptionsChanged;
        }

        public void Add(Exception exception)
            => Threads.RunInMainThread(() => { exceptions.Add(exception); });

        public void Clear()
            => Threads.RunInMainThread(exceptions.Clear);

        private void OnExceptionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Last = exceptions.LastOrDefault();
            OnPropertyChanged(nameof(Exceptions));
        }
    }
}
