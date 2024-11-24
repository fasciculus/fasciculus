using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Assets.Services;
using Fasciculus.Maui;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Windows.Input;

namespace Fasciculus.Eve.Assets.PageModels
{
    public partial class MainPageModel : ObservableObject
    {
        private IProgressCollector progressCollector;

        private readonly ILogger logger;

        [ObservableProperty]
        private string downloadSdeStatusText = "Pending";

        [ObservableProperty]
        private Color downloadSdeStatusColor = Colors.Orange;

        [ObservableProperty]
        private double extractSdeProgressValue = 0;

        [ObservableProperty]
        private Color extractSdeProgressColor = Colors.Orange;

        public ICommand StartCommand { get; init; }

        public MainPageModel(IProgressCollector progressCollector, IExtractSde extractSde, ILogger<MainPageModel> logger)
        {
            this.progressCollector = progressCollector;
            this.progressCollector.PropertyChanged += OnProgressChanged;

            StartCommand = new LongRunningCommand(() => extractSde.Extract());

            this.logger = logger;
        }

        private void OnProgressChanged(object? sender, PropertyChangedEventArgs ev)
        {
            logger.LogInformation($"{MainThread.IsMainThread}");
            MainThread.BeginInvokeOnMainThread(() => OnProgressChanged(ev.PropertyName ?? string.Empty));
        }

        private void OnProgressChanged(string name)
        {
            logger.LogInformation($"{DateTime.UtcNow.Millisecond} {name}");

            switch (name)
            {
                case nameof(IProgressCollector.DownloadSdeStatus):
                    OnDownloadSdeStatusChanged();
                    break;

                case nameof(IProgressCollector.ExtractSdeProgress):
                    OnExtractSdeProgressChanged();
                    break;
            }
        }

        private void OnDownloadSdeStatusChanged()
        {
            DownloadSdeStatus status = progressCollector.DownloadSdeStatus;

            DownloadSdeStatusText = status switch
            {
                DownloadSdeStatus.Pending => "Pending",
                DownloadSdeStatus.Downloading => "Downloading",
                DownloadSdeStatus.Downloaded => "Downloaded",
                DownloadSdeStatus.NotModified => "Not Modified",
                _ => string.Empty
            };

            DownloadSdeStatusColor = status switch
            {
                DownloadSdeStatus.Pending => Colors.Orange,
                DownloadSdeStatus.Downloading => Colors.Orange,
                DownloadSdeStatus.Downloaded => Colors.Green,
                DownloadSdeStatus.NotModified => Colors.Green,
                _ => Colors.Black
            };
        }

        private void OnExtractSdeProgressChanged()
        {
            ExtractSdeProgressValue = progressCollector.ExtractSdeProgress;
            ExtractSdeProgressColor = progressCollector.ExtractSdeProgress == 1.0 ? Colors.Green : Colors.Orange;
        }
    }
}
