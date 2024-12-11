using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using System.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class StatusBarModel : MainThreadObservable
    {
        private const string NoError = "No Error";

        private readonly ILastError lastError;

        [ObservableProperty]
        private string text = NoError;

        [ObservableProperty]
        private bool isError = false;

        public StatusBarModel(ILastError lastError)
        {
            this.lastError = lastError;
            this.lastError.PropertyChanged += OnLastErrorChanged;
        }

        private void OnLastErrorChanged(object? sender, PropertyChangedEventArgs e)
        {
            Exception? error = lastError.Error;

            Text = error?.Message ?? NoError;
            IsError = error is not null;
        }
    }
}
