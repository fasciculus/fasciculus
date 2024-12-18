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
        public partial DownloadSdeStatus DownloadSde { get; private set; }

        [ObservableProperty]
        public partial LongProgressInfo ExtractSde { get; private set; }

        [ObservableProperty]
        public partial WorkState ParseNames { get; private set; }

        [ObservableProperty]
        public partial WorkState ParseMarketGroups { get; private set; }

        [ObservableProperty]
        public partial WorkState ParseTypes { get; private set; }

        [ObservableProperty]
        public partial WorkState ParseStationOperations { get; private set; }

        [ObservableProperty]
        public partial WorkState ParseNpcCorporations { get; private set; }

        [ObservableProperty]
        public partial WorkState ParsePlanetSchematics { get; private set; }

        [ObservableProperty]
        public partial WorkState ParseBlueprints { get; private set; }

        [ObservableProperty]
        public partial LongProgressInfo ParseRegions { get; private set; }

        [ObservableProperty]
        public partial LongProgressInfo ParseConstellations { get; private set; }

        [ObservableProperty]
        public partial LongProgressInfo ParseSolarSystems { get; private set; }

        [ObservableProperty]
        public partial WorkState ConvertData { get; private set; }

        [ObservableProperty]
        public partial WorkState ConvertUniverse { get; private set; }

        [ObservableProperty]
        public partial WorkState CreateConnections { get; private set; }

        [ObservableProperty]
        public partial LongProgressInfo CreateDistances { get; private set; }

        [ObservableProperty]
        public partial LongProgressInfo CopyImages { get; private set; }

        [ObservableProperty]
        public partial WorkState CreateImages { get; private set; }

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

            DownloadSde = DownloadSdeStatus.Pending;
            ExtractSde = LongProgressInfo.Start;

            ParseNames = WorkState.Pending;
            ParseMarketGroups = WorkState.Pending;
            ParseTypes = WorkState.Pending;
            ParseStationOperations = WorkState.Pending;
            ParseNpcCorporations = WorkState.Pending;
            ParsePlanetSchematics = WorkState.Pending;
            ParseBlueprints = WorkState.Pending;

            ParseRegions = LongProgressInfo.Start;
            ParseConstellations = LongProgressInfo.Start;
            ParseSolarSystems = LongProgressInfo.Start;

            ConvertData = WorkState.Pending;
            ConvertUniverse = WorkState.Pending;

            CreateConnections = WorkState.Pending;
            CreateDistances = LongProgressInfo.Start;

            CopyImages = LongProgressInfo.Start;
            CreateImages = WorkState.Pending;
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
            ParsePlanetSchematics = assetsProgress.ParsePlanetSchematicsInfo;
            ParseBlueprints = assetsProgress.ParseBlueprintsInfo;

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
            int len = assetsDirectories.Resources.FullName.Length + 1;

            List<string> current = [.. assetsProgress.CreateResourcesInfo.Select(x => x.FullName[len..].Replace('\\', '/')).OrderBy(x => x)];
            List<string> existing = new(ChangedResources);
            List<string> remove = existing.Where(x => !current.Contains(x)).ToList();
            List<string> add = current.Where(x => !existing.Contains(x)).ToList();

            remove.Apply(x => { ChangedResources.Remove(x); });
            add.Apply(ChangedResources.Add);
        }

        [RelayCommand]
        private Task Start()
        {
            return createResources.CreateAsync();
        }
    }
}
