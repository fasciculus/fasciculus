﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Assets.Services;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace Fasciculus.Eve.Assets.PageModels
{
    public partial class MainPageModel : ObservableObject
    {
        private static readonly string PendingText = "Pending";
        private static readonly string WorkingText = "Working";
        private static readonly string DoneText = "Done";

        private static readonly Color PendingColor = Colors.Orange;
        private static readonly Color WorkingColor = Colors.Orange;
        private static readonly Color DoneColor = Colors.Green;

        private readonly IProgressCollector progressCollector;
        private readonly IAssetsDirectories assetsDirectories;
        private readonly ICreateResources createResources;

        [ObservableProperty]
        private string downloadSdeText = PendingText;

        [ObservableProperty]
        private Color downloadSdeColor = PendingColor;

        [ObservableProperty]
        private double extractSdeValue = 0;

        [ObservableProperty]
        private Color extractSdeColor = PendingColor;

        [ObservableProperty]
        private string parseNamesText = PendingText;

        [ObservableProperty]
        private Color parseNamesColor = PendingColor;

        [ObservableProperty]
        private string parseTypesText = PendingText;

        [ObservableProperty]
        private Color parseTypesColor = PendingColor;

        [ObservableProperty]
        private double parseRegionsValue = 0;

        [ObservableProperty]
        private Color parseRegionsColor = PendingColor;

        [ObservableProperty]
        private double parseConstellationsValue = 0;

        [ObservableProperty]
        private Color parseConstellationsColor = PendingColor;

        [ObservableProperty]
        private double parseSolarSystemsValue = 0;

        [ObservableProperty]
        private Color parseSolarSystemsColor = PendingColor;

        [ObservableProperty]
        private string convertUniverseText = PendingText;

        [ObservableProperty]
        private Color convertUniverseColor = PendingColor;

        [ObservableProperty]
        private double copyImagesValue = 0;

        [ObservableProperty]
        private Color copyImagesColor = PendingColor;

        [ObservableProperty]
        private string[] changedResources = [];

        private ILogger logger;

        public MainPageModel(IProgressCollector progressCollector, IAssetsDirectories assetsDirectories, ICreateResources createResources,
            ILogger<MainPageModel> logger)
        {
            this.progressCollector = progressCollector;
            this.progressCollector.PropertyChanged += OnProgressChanged;
            this.assetsDirectories = assetsDirectories;
            this.createResources = createResources;
            this.logger = logger;
        }

        private void OnProgressChanged(object? sender, PropertyChangedEventArgs ev)
        {
            DownloadSdeText = DownloadSdeStatusText(progressCollector.DownloadSde);
            DownloadSdeColor = DownloadSdeStatusColor(progressCollector.DownloadSde);

            ExtractSdeValue = progressCollector.ExtractSde;
            ExtractSdeColor = progressCollector.ExtractSde == 1.0 ? DoneColor : PendingColor;

            ParseNamesText = PendingToDoneText(progressCollector.ParseNames);
            ParseNamesColor = PendingToDoneColor(progressCollector.ParseNames);

            ParseTypesText = PendingToDoneText(progressCollector.ParseTypes);
            ParseTypesColor = PendingToDoneColor(progressCollector.ParseTypes);

            ParseRegionsValue = progressCollector.ParseRegions;
            ParseRegionsColor = progressCollector.ParseRegionsDone ? DoneColor : PendingColor;

            ParseConstellationsValue = progressCollector.ParseConstellations;
            ParseConstellationsColor = progressCollector.ParseConstellationsDone ? DoneColor : PendingColor;

            ParseSolarSystemsValue = progressCollector.ParseSolarSystems;
            ParseSolarSystemsColor = progressCollector.ParseSolarSystemsDone ? DoneColor : PendingColor;

            ConvertUniverseText = PendingToDoneText(progressCollector.ConvertUniverse);
            ConvertUniverseColor = PendingToDoneColor(progressCollector.ConvertUniverse);

            CopyImagesValue = progressCollector.CopyImages;
            CopyImagesColor = progressCollector.CopyImages == 1.0 ? DoneColor : PendingColor;

            int prefix = assetsDirectories.Resources.FullName.Length + 1;

            ChangedResources = progressCollector.ChangedResources
                .Select(x => x.FullName.Substring(prefix).Replace('\\', '/'))
                .ToArray();
        }

        private static string DownloadSdeStatusText(DownloadSdeStatus status)
        {
            return status switch
            {
                DownloadSdeStatus.Pending => "Pending",
                DownloadSdeStatus.Downloading => "Downloading",
                DownloadSdeStatus.Downloaded => "Downloaded",
                DownloadSdeStatus.NotModified => "Not Modified",
                _ => string.Empty
            };
        }

        private static Color DownloadSdeStatusColor(DownloadSdeStatus status)
        {
            return status switch
            {
                DownloadSdeStatus.Pending => PendingColor,
                DownloadSdeStatus.Downloading => PendingColor,
                DownloadSdeStatus.Downloaded => DoneColor,
                DownloadSdeStatus.NotModified => DoneColor,
                _ => Colors.Black
            };

        }

        private static string PendingToDoneText(PendingToDone status)
        {
            return status switch
            {
                PendingToDone.Pending => PendingText,
                PendingToDone.Working => WorkingText,
                PendingToDone.Done => DoneText,
                _ => string.Empty
            };
        }

        private static Color PendingToDoneColor(PendingToDone status)
        {
            return status switch
            {
                PendingToDone.Pending => PendingColor,
                PendingToDone.Working => WorkingColor,
                PendingToDone.Done => DoneColor,
                _ => Colors.Red
            };
        }

        [RelayCommand]
        private Task Start()
        {
            return createResources.CreateAsync();
        }
    }
}
