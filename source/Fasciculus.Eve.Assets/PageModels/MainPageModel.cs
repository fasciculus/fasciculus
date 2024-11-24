using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Assets.Services;
using Fasciculus.Maui;
using System.ComponentModel;
using System.Windows.Input;

namespace Fasciculus.Eve.Assets.PageModels
{
    public partial class MainPageModel : ObservableObject
    {
        private IProgressCollector progressCollector;

        [ObservableProperty]
        private string downloadSdeStatusText = "Pending";

        [ObservableProperty]
        private Color downloadSdeStatusColor = PendingOrDoneColor(PendingOrDone.Pending);

        [ObservableProperty]
        private double extractSdeProgressValue = 0;

        [ObservableProperty]
        private Color extractSdeProgressColor = PendingOrDoneColor(PendingOrDone.Pending);

        [ObservableProperty]
        private string parseNamesText = PendingOrDoneText(PendingOrDone.Pending);

        [ObservableProperty]
        private Color parseNamesColor = PendingOrDoneColor(PendingOrDone.Pending);

        public ICommand StartCommand { get; init; }

        public MainPageModel(IProgressCollector progressCollector, IParseData parseData)
        {
            this.progressCollector = progressCollector;
            this.progressCollector.PropertyChanged += OnProgressChanged;

            StartCommand = new LongRunningCommand(() => parseData.Parse());
        }

        private void OnProgressChanged(object? sender, PropertyChangedEventArgs ev)
        {
            MainThread.BeginInvokeOnMainThread(() => OnProgressChanged(ev.PropertyName ?? string.Empty));
        }

        private void OnProgressChanged(string name)
        {
            switch (name)
            {
                case nameof(IProgressCollector.DownloadSde):
                    OnDownloadSdeChanged();
                    break;

                case nameof(IProgressCollector.ExtractSde):
                    OnExtractSdeChanged();
                    break;

                case nameof(IProgressCollector.ParseNames):
                    OnParseNamesChanged();
                    break;
            }
        }

        private void OnDownloadSdeChanged()
        {
            DownloadSdeStatus status = progressCollector.DownloadSde;

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

        private void OnExtractSdeChanged()
        {
            ExtractSdeProgressValue = progressCollector.ExtractSde;
            ExtractSdeProgressColor = progressCollector.ExtractSde == 1.0 ? Colors.Green : Colors.Orange;
        }

        private void OnParseNamesChanged()
        {
            ParseNamesText = PendingOrDoneText(progressCollector.ParseNames);
            ParseNamesColor = PendingOrDoneColor(progressCollector.ParseNames);
        }

        private static string PendingOrDoneText(PendingOrDone status)
        {
            return status switch
            {
                PendingOrDone.Pending => "Pending",
                PendingOrDone.Done => "Done",
                _ => string.Empty
            };
        }

        private static Color PendingOrDoneColor(PendingOrDone status)
        {
            return status switch
            {
                PendingOrDone.Pending => Colors.Orange,
                PendingOrDone.Done => Colors.Green,
                _ => Colors.Red
            };
        }
    }
}
