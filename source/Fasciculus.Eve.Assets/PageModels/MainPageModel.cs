using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Assets.Services;
using Fasciculus.Maui.Support.Progressing;
using Fasciculus.Support;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Fasciculus.Eve.Assets.PageModels
{
    public class NotifyingCollection<T> : ObservableCollection<T>
    {
        public NotifyingCollection(IEnumerable<T> values, INotifyCollectionChanged notifier)
            : base(values)
        {
            notifier.CollectionChanged += Notifier_CollectionChanged;
        }

        private void Notifier_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Add(e.NewItems, e.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    Remove(e.OldItems, e.OldStartingIndex);
                    break;
            }
        }

        private void Add(IList? items, int startingIndex)
        {
            if (items is null) { return; }

            for (int i = 0; i < items.Count; ++i)
            {
                T item = (T)Cond.NotNull(items[i]);

                InsertItem(startingIndex + i, item);
            }
        }

        private void Remove(IList? oldItems, int startingIndex)
        {
            throw Ex.NotImplemented();
        }
    }

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

        public ChangedResourcesSet ChangedResources { get; }
        public ObservableCollection<string> ChangedResources2 { get; }
        public NotifyingCollection<string> ChangedResources3 { get; }

        public CollectionView? CV1 { get; set; }
        public CollectionView? CV2 { get; set; }

        public MainPageModel(IAssetsProgress assetsProgress, ICreateResources createResources, ChangedResourcesSet changedResources)
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

            ChangedResources = changedResources;
            ChangedResources.CollectionChanged += ChangedResources_CollectionChanged;

            ChangedResources2 = [];
            ChangedResources3 = new(ChangedResources, ChangedResources);
        }

        private void ChangedResources_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ChangedResources2.Clear();
            //ChangedResources3.Clear();

            ChangedResources.Apply(ChangedResources2.Add);
            //ChangedResources.Apply(ChangedResources3.Add);
        }

        [RelayCommand]
        private Task Start()
        {
            return createResources.CreateAsync();
        }
    }
}
