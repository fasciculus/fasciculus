using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Assets.Services;
using Fasciculus.Support;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Windows.Input;

namespace Fasciculus.Eve.Assets.PageModels
{
    public partial class MainPageModel : ObservableObject
    {
        private static readonly Color PendingColor = Colors.Orange;
        private static readonly Color DoneColor = Colors.Green;

        private readonly IProgressCollector progressCollector;
        private readonly IAssetsDirectories assetsDirectories;

        [ObservableProperty]
        private string downloadSdeText = PendingOrDoneText(PendingOrDone.Pending);

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
        private double parseRegionsValue = 0;

        [ObservableProperty]
        private Color parseRegionsColor = PendingOrDoneColor(PendingOrDone.Pending);

        [ObservableProperty]
        private double parseConstellationsValue = 0;

        [ObservableProperty]
        private Color parseConstellationsColor = PendingOrDoneColor(PendingOrDone.Pending);

        [ObservableProperty]
        private double parseSolarSystemsValue = 0;

        [ObservableProperty]
        private Color parseSolarSystemsColor = PendingOrDoneColor(PendingOrDone.Pending);

        [ObservableProperty]
        private double copyImagesValue = 0;

        [ObservableProperty]
        private Color copyImagesColor = PendingOrDoneColor(PendingOrDone.Pending);

        [ObservableProperty]
        private string[] changedResources = [];

        public ICommand StartCommand { get; init; }

        private ILogger logger;

        public MainPageModel(IProgressCollector progressCollector, IAssetsDirectories assetsDirectories, IResourcesCreator resourcesCreator, ILogger<MainPageModel> logger)
        {
            this.progressCollector = progressCollector;
            this.progressCollector.PropertyChanged += OnProgressChanged;
            this.assetsDirectories = assetsDirectories;

            StartCommand = new LongRunningCommand(() => resourcesCreator.Create());

            this.logger = logger;
        }

        private void OnProgressChanged(object? sender, PropertyChangedEventArgs ev)
        {
            DownloadSdeText = DownloadSdeStatusText(progressCollector.DownloadSde);
            DownloadSdeColor = DownloadSdeStatusColor(progressCollector.DownloadSde);

            ExtractSdeValue = progressCollector.ExtractSde;
            ExtractSdeColor = progressCollector.ExtractSde == 1.0 ? DoneColor : PendingColor;

            ParseNamesText = PendingOrDoneText(progressCollector.ParseNames);
            ParseNamesColor = PendingOrDoneColor(progressCollector.ParseNames);

            ParseTypesText = PendingOrDoneText(progressCollector.ParseTypes);
            ParseTypesColor = PendingOrDoneColor(progressCollector.ParseTypes);

            ParseRegionsValue = progressCollector.ParseRegions;
            ParseRegionsColor = progressCollector.ParseRegions == 1.0 ? DoneColor : PendingColor;

            ParseConstellationsValue = progressCollector.ParseConstellations;
            ParseConstellationsColor = progressCollector.ParseConstellations == 1.0 ? DoneColor : PendingColor;

            ParseSolarSystemsValue = progressCollector.ParseSolarSystems;
            ParseSolarSystemsColor = progressCollector.ParseSolarSystems == 1.0 ? DoneColor : PendingColor;

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
