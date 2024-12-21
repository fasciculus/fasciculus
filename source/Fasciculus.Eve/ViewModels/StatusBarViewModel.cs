using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Services;
using System.ComponentModel;

namespace Fasciculus.Eve.ViewModels
{
    public partial class StatusBarViewModel : MainThreadObservable
    {
        private const string NoError = "No Error";

        private readonly IExceptionCollector exceptions;

        [ObservableProperty]
        public partial string Text { get; private set; }

        [ObservableProperty]
        public partial bool IsError { get; private set; }

        public StatusBarViewModel(IExceptionCollector exceptions)
        {
            this.exceptions = exceptions;
            this.exceptions.PropertyChanged += OnExceptionsChanged;

            Text = NoError;
            IsError = false;
        }

        private void OnExceptionsChanged(object? sender, PropertyChangedEventArgs e)
        {
            Exception? exception = exceptions.Last;

            Text = exception?.Message ?? NoError;
            IsError = exception is not null;
        }

        [RelayCommand]
        private void Clear()
        {
            exceptions.Clear();
        }
    }
}
