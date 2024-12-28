using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Assets.Services;
using Fasciculus.Maui.Collections;
using Fasciculus.Maui.Support.Progressing;

namespace Fasciculus.Eve.Assets.PageModels
{
    public partial class MainPageModel : ObservableObject
    {
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

        public MainThreadNotifyingEnumerable<string> ChangedResources { get; }

        public MainPageModel(ICreateResources createResources, IAssetsProgress assetsProgress, ChangedResourcesSet changedResources)
        {
            this.createResources = createResources;

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

            ChangedResources = new(changedResources);
        }

        [RelayCommand]
        private Task Start()
        {
            return createResources.CreateAsync();
        }
    }
}
