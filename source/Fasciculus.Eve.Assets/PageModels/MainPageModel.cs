using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Assets.Services;
using Fasciculus.Maui.Support;
using Fasciculus.Support;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Fasciculus.Eve.Assets.PageModels
{
    public partial class MainPageModel : ObservableObject
    {
        private readonly IAssetsProgress assetsProgress;
        private readonly IAssetsDirectories assetsDirectories;
        private readonly ICreateResources createResources;

        [ObservableProperty]
        private DownloadSdeStatus downloadSde = DownloadSdeStatus.Pending;

        [ObservableProperty]
        private LongProgressInfo extractSde = LongProgressInfo.Start;

        [ObservableProperty]
        private WorkState parseNames = WorkState.Pending;

        [ObservableProperty]
        private WorkState parseMarketGroups = WorkState.Pending;

        [ObservableProperty]
        private WorkState parseTypes = WorkState.Pending;

        [ObservableProperty]
        private WorkState parseStationOperations = WorkState.Pending;

        [ObservableProperty]
        private WorkState parseNpcCorporations = WorkState.Pending;

        [ObservableProperty]
        private LongProgressInfo parseRegions = LongProgressInfo.Start;

        [ObservableProperty]
        private LongProgressInfo parseConstellations = LongProgressInfo.Start;

        [ObservableProperty]
        private LongProgressInfo parseSolarSystems = LongProgressInfo.Start;

        [ObservableProperty]
        private WorkState convertData = WorkState.Pending;

        [ObservableProperty]
        private WorkState convertUniverse = WorkState.Pending;

        [ObservableProperty]
        private WorkState createConnections = WorkState.Pending;

        [ObservableProperty]
        private LongProgressInfo createDistances = LongProgressInfo.Start;

        [ObservableProperty]
        private LongProgressInfo copyImages = LongProgressInfo.Start;

        [ObservableProperty]
        private WorkState createImages = WorkState.Pending;

        public ObservableCollection<string> ChangedResources { get; } = [];

        private ILogger logger;

        public MainPageModel(IAssetsProgress assetsProgress, IAssetsDirectories assetsDirectories, ICreateResources createResources,
            ILogger<MainPageModel> logger)
        {
            this.assetsProgress = assetsProgress;
            this.assetsProgress.PropertyChanged += OnProgressChanged;

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

            ConvertData = assetsProgress.ConvertDataInfo;
            ConvertUniverse = assetsProgress.ConvertUniverseInfo;

            CreateConnections = assetsProgress.CreateConnectionsInfo;
            CreateDistances = assetsProgress.CreateDistancesInfo;

            CopyImages = assetsProgress.CopyImagesInfo;
            CreateImages = assetsProgress.CreateImagesInfo;

            if (ev.PropertyName == nameof(IAssetsProgress.CreateResourcesInfo))
            {
                UpdateChangedResources();
            }
        }

        private void UpdateChangedResources()
        {
            int prefixLength = assetsDirectories.Resources.FullName.Length + 1;

            ChangedResources.Clear();

            assetsProgress.CreateResourcesInfo
                .Select(x => x.FullName[prefixLength..].Replace('\\', '/'))
                .Apply(ChangedResources.Add);
        }

        [RelayCommand]
        private Task Start()
        {
            return createResources.CreateAsync();
        }
    }
}
