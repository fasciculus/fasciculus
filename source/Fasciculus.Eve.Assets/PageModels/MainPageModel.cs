using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Assets.Services;
using Fasciculus.Support;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace Fasciculus.Eve.Assets.PageModels
{
    public partial class MainPageModel : ObservableObject
    {
        private readonly IAssetsProgress assetsProgress;
        private readonly IProgressCollector progressCollector;
        private readonly IAssetsDirectories assetsDirectories;
        private readonly ICreateResources createResources;

        [ObservableProperty]
        private DownloadSdeStatus downloadSde = DownloadSdeStatus.Pending;

        [ObservableProperty]
        private LongProgressInfo extractSde = LongProgressInfo.Start;

        [ObservableProperty]
        private PendingToDone parseNames = PendingToDone.Pending;

        [ObservableProperty]
        private PendingToDone parseMarketGroups = PendingToDone.Pending;

        [ObservableProperty]
        private PendingToDone parseTypes = PendingToDone.Pending;

        [ObservableProperty]
        private PendingToDone parseStationOperations = PendingToDone.Pending;

        [ObservableProperty]
        private PendingToDone parseNpcCorporations = PendingToDone.Pending;

        [ObservableProperty]
        private LongProgressInfo parseRegions = LongProgressInfo.Start;

        [ObservableProperty]
        private LongProgressInfo parseConstellations = LongProgressInfo.Start;

        [ObservableProperty]
        private LongProgressInfo parseSolarSystems = LongProgressInfo.Start;

        [ObservableProperty]
        private PendingToDone convertData = PendingToDone.Pending;

        [ObservableProperty]
        private PendingToDone convertUniverse = PendingToDone.Pending;

        [ObservableProperty]
        private PendingToDone createConnections = PendingToDone.Pending;

        [ObservableProperty]
        private LongProgressInfo createDistances = LongProgressInfo.Start;

        [ObservableProperty]
        private LongProgressInfo copyImages = LongProgressInfo.Start;

        [ObservableProperty]
        private PendingToDone createImages = PendingToDone.Pending;

        [ObservableProperty]
        private string[] changedResources = [];

        private ILogger logger;

        public MainPageModel(IAssetsProgress assetsProgress, IProgressCollector progressCollector, IAssetsDirectories assetsDirectories, ICreateResources createResources,
            ILogger<MainPageModel> logger)
        {
            this.assetsProgress = assetsProgress;
            this.assetsProgress.PropertyChanged += OnProgressChanged;

            this.progressCollector = progressCollector;
            this.progressCollector.PropertyChanged += OnProgressChanged;

            this.assetsDirectories = assetsDirectories;
            this.createResources = createResources;
            this.logger = logger;
        }

        private void OnProgressChanged(object? sender, PropertyChangedEventArgs ev)
        {
            DownloadSde = assetsProgress.DownloadSdeInfo;
            ExtractSde = assetsProgress.ExtractSdeInfo;

            ParseNames = assetsProgress.ParseNamesInfo;
            ParseMarketGroups = assetsProgress.ParseMarketGroupsInfo;
            ParseTypes = assetsProgress.ParseTypesInfo;
            ParseStationOperations = assetsProgress.ParseStationOperationsInfo;
            ParseNpcCorporations = assetsProgress.ParseNpcCorporationsInfo;

            ParseRegions = assetsProgress.ParseRegionsInfo;
            ParseConstellations = assetsProgress.ParseConstellationsInfo;
            ParseSolarSystems = assetsProgress.ParseSolarSystemsInfo;

            ConvertData = progressCollector.ConvertData;
            ConvertUniverse = progressCollector.ConvertUniverse;

            CreateConnections = progressCollector.CreateConnections;
            CreateDistances = progressCollector.CreateDistances;

            CopyImages = progressCollector.CopyImages;
            CreateImages = progressCollector.CreateImages;

            ChangedResources = GetChangedResources();
        }

        private string[] GetChangedResources()
        {
            int prefix = assetsDirectories.Resources.FullName.Length + 1;

            return progressCollector.ChangedResources
                .Select(x => x.FullName[prefix..].Replace('\\', '/'))
                .ToArray();
        }

        [RelayCommand]
        private Task Start()
        {
            return createResources.CreateAsync();
        }
    }
}
