﻿using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Assets.Services;
using Fasciculus.Maui.Input;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Windows.Input;

namespace Fasciculus.Eve.Assets.PageModels
{
    public partial class MainPageModel : ObservableObject
    {
        private static Color PendingColor = Colors.Orange;
        private static Color DoneColor = Colors.Green;

        private IProgressCollector progressCollector;

        [ObservableProperty]
        private string downloadSdeText = "Pending";

        [ObservableProperty]
        private Color downloadSdeColor = PendingOrDoneColor(PendingOrDone.Pending);

        [ObservableProperty]
        private double extractSdeValue = 0;

        [ObservableProperty]
        private Color extractSdeColor = PendingOrDoneColor(PendingOrDone.Pending);

        [ObservableProperty]
        private string parseNamesText = PendingOrDoneText(PendingOrDone.Pending);

        [ObservableProperty]
        private Color parseNamesColor = PendingOrDoneColor(PendingOrDone.Pending);

        [ObservableProperty]
        private string parseTypesText = PendingOrDoneText(PendingOrDone.Pending);

        [ObservableProperty]
        private Color parseTypesColor = PendingOrDoneColor(PendingOrDone.Pending);

        [ObservableProperty]
        private double copyImagesValue = 0;

        [ObservableProperty]
        private Color copyImagesColor = PendingOrDoneColor(PendingOrDone.Pending);

        public ICommand StartCommand { get; init; }

        private ILogger logger;

        public MainPageModel(IProgressCollector progressCollector, IResourcesCreator resourcesCreator, ILogger<MainPageModel> logger)
        {
            this.progressCollector = progressCollector;
            this.progressCollector.PropertyChanged += OnProgressChanged;

            StartCommand = new LongRunningCommand(() => resourcesCreator.Create());

            this.logger = logger;
        }

        private void OnProgressChanged(object? sender, PropertyChangedEventArgs ev)
        {
            //logger.LogInformation("{name}: {isMainThread}", ev.PropertyName, MainThread.IsMainThread);

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

                case nameof(IProgressCollector.CopyImages):
                    OnCopyImagesChanged();
                    break;
            }
        }

        private void OnDownloadSdeChanged()
        {
            DownloadSdeStatus status = progressCollector.DownloadSde;

            DownloadSdeText = status switch
            {
                DownloadSdeStatus.Pending => "Pending",
                DownloadSdeStatus.Downloading => "Downloading",
                DownloadSdeStatus.Downloaded => "Downloaded",
                DownloadSdeStatus.NotModified => "Not Modified",
                _ => string.Empty
            };

            DownloadSdeColor = status switch
            {
                DownloadSdeStatus.Pending => PendingColor,
                DownloadSdeStatus.Downloading => PendingColor,
                DownloadSdeStatus.Downloaded => DoneColor,
                DownloadSdeStatus.NotModified => DoneColor,
                _ => Colors.Black
            };
        }

        private void OnExtractSdeChanged()
        {
            ExtractSdeValue = progressCollector.ExtractSde;
            ExtractSdeColor = progressCollector.ExtractSde == 1.0 ? DoneColor : PendingColor;
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

        private void OnCopyImagesChanged()
        {
            CopyImagesValue = progressCollector.CopyImages;
            CopyImagesColor = progressCollector.CopyImages == 1.0 ? DoneColor : PendingColor;
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
                PendingOrDone.Pending => PendingColor,
                PendingOrDone.Done => DoneColor,
                _ => Colors.Red
            };
        }
    }
}
