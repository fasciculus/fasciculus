using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Assets.Services;
using Fasciculus.Maui.Support.Progressing;
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

        public ProgressBarProgress DownloadSde { get; }
        public ProgressBarProgress ExtractSde { get; }

        public ProgressBarProgress ParseNames { get; }
        public ProgressBarProgress ParseMarketGroups { get; }
        public ProgressBarProgress ParseTypes { get; }
        public ProgressBarProgress ParseStationOperations { get; }
        public ProgressBarProgress ParseNpcCorporations { get; }
        public ProgressBarProgress ParsePlanetSchematics { get; }
        public ProgressBarProgress ParseBlueprints { get; }

        public ProgressBarProgress ParseRegions { get; }
        public ProgressBarProgress ParseConstellations { get; }
        public ProgressBarProgress ParseSolarSystems { get; }

        public ProgressBarProgress ConvertData { get; }
        public ProgressBarProgress ConvertUniverse { get; }

        public ProgressBarProgress CreateConnections { get; }
        public ProgressBarProgress CreateDistances { get; }

        public ProgressBarProgress CopyImages { get; }
        public ProgressBarProgress CreateImages { get; }

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

            DownloadSde = assetsProgress.DownloadSde;
            ExtractSde = assetsProgress.ExtractSde;

            ParseNames = assetsProgress.ParseNames;
            ParseMarketGroups = assetsProgress.ParseMarketGroups;
            ParseTypes = assetsProgress.ParseTypes;
            ParseStationOperations = assetsProgress.ParseStationOperations;
            ParseNpcCorporations = assetsProgress.ParseNpcCorporations;
            ParsePlanetSchematics = assetsProgress.ParsePlanetSchematics;
            ParseBlueprints = assetsProgress.ParseBlueprints;

            ParseRegions = assetsProgress.ParseRegions;
            ParseConstellations = assetsProgress.ParseConstellations;
            ParseSolarSystems = assetsProgress.ParseSolarSystems;

            ConvertData = assetsProgress.ConvertData;
            ConvertUniverse = assetsProgress.ConvertUniverse;

            CreateConnections = assetsProgress.CreateConnections;
            CreateDistances = assetsProgress.CreateDistances;

            CopyImages = assetsProgress.CopyImages;
            CreateImages = assetsProgress.CreateImages;
        }

        private void OnProgressChanged(object? sender, PropertyChangedEventArgs ev)
        {
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
