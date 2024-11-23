using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Assets.Services;
using System.ComponentModel;

namespace Fasciculus.Eve.Assets.PageModels
{
    public partial class MainPageModel : ObservableObject
    {
        private IProgressCollector progressCollector;
        private IDownloadSde downloadSde;

        public MainPageModel(IProgressCollector progressCollector, IDownloadSde downloadSde)
        {
            this.progressCollector = progressCollector;
            this.progressCollector.PropertyChanged += OnProgressChanged;

            this.downloadSde = downloadSde;

            OnDownloadSdeStatusChanged();
        }

        [ObservableProperty]
        private bool startEnabled = true;

        [ObservableProperty]
        private string downloadStatusText = string.Empty;

        [ObservableProperty]
        private Color downloadStatusColor = Colors.Black;

        [RelayCommand]
        private void Start()
        {
            StartEnabled = false;

            downloadSde.DownloadAsync();
        }

        private void OnProgressChanged(object? sender, PropertyChangedEventArgs ev)
        {
            MainThread.BeginInvokeOnMainThread(() => OnProgressChanged(ev.PropertyName ?? string.Empty));
        }

        private void OnProgressChanged(string name)
        {
            switch (name)
            {
                case nameof(IProgressCollector.DownloadSdeStatus):
                    OnDownloadSdeStatusChanged();
                    break;
            }
        }

        private void OnDownloadSdeStatusChanged()
        {
            DownloadSdeStatus status = progressCollector.DownloadSdeStatus;

            DownloadStatusText = status switch
            {
                DownloadSdeStatus.Pending => "Pending",
                DownloadSdeStatus.Downloading => "Downloading",
                DownloadSdeStatus.Downloaded => "Downloaded",
                DownloadSdeStatus.NotModified => "Not Modified",
                _ => string.Empty
            };

            DownloadStatusColor = status switch
            {
                DownloadSdeStatus.Pending => Colors.Orange,
                DownloadSdeStatus.Downloading => Colors.Orange,
                DownloadSdeStatus.Downloaded => Colors.Green,
                DownloadSdeStatus.NotModified => Colors.Green,
                _ => Colors.Black
            };
        }
    }
}
