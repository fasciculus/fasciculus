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

        [ObservableProperty]
        private string parseTypesText = PendingOrDoneText(PendingOrDone.Pending);

        [ObservableProperty]
        private Color parseTypesColor = PendingOrDoneColor(PendingOrDone.Pending);

        public ICommand StartCommand { get; init; }

        private ILogger logger;

        public MainPageModel(IProgressCollector progressCollector, IDataParser dataParser, ILogger<MainPageModel> logger)
        {
            this.progressCollector = progressCollector;
            this.progressCollector.PropertyChanged += OnProgressChanged;

            StartCommand = new LongRunningCommand(() => dataParser.Parse());

            this.logger = logger;
        }

        private void OnProgressChanged(object? sender, PropertyChangedEventArgs ev)
        {
            logger.LogInformation("{name}: {isMainThread}", ev.PropertyName, MainThread.IsMainThread);

            switch (ev.PropertyName ?? string.Empty)
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

                case nameof(IProgressCollector.ParseTypes):
                    OnParseTypesChanged();
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

        private void OnParseTypesChanged()
        {
            ParseTypesText = PendingOrDoneText(progressCollector.ParseTypes);
            ParseTypesColor = PendingOrDoneColor(progressCollector.ParseTypes);
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
