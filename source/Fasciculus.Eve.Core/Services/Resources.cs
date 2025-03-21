using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.IO.Resources;
using Fasciculus.Maui.Support;
using Fasciculus.Threading;
using Fasciculus.Threading.Synchronization;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Services
{
    public interface IEveResourcesProgress : INotifyPropertyChanged
    {
        public WorkState DataInfo { get; }
        public IProgress<WorkState> DataProgress { get; }

        public WorkState UniverseInfo { get; }
        public IProgress<WorkState> UniverseProgress { get; }

        public WorkState NavigationInfo { get; }
        public IProgress<WorkState> NavigationProgress { get; }
    }

    public partial class EveResourcesProgress : ObservableObject, IEveResourcesProgress
    {
        [ObservableProperty]
        public partial WorkState DataInfo { get; set; }

        public IProgress<WorkState> DataProgress { get; }

        [ObservableProperty]
        public partial WorkState UniverseInfo { get; set; }

        public IProgress<WorkState> UniverseProgress { get; }

        [ObservableProperty]
        public partial WorkState NavigationInfo { get; set; }

        public IProgress<WorkState> NavigationProgress { get; }

        [ObservableProperty]
        public partial bool Universe { get; set; }

        [ObservableProperty]
        public partial bool Navigation { get; set; }

        public EveResourcesProgress()
        {
            DataProgress = new WorkStateProgress((x) => { DataInfo = x; });
            UniverseProgress = new WorkStateProgress((x) => { UniverseInfo = x; });
            NavigationProgress = new WorkStateProgress((x) => { NavigationInfo = x; });
        }
    }

    public interface IEveResources
    {
        public Task<EveData> Data { get; }
        public Task<EveUniverse> Universe { get; }
        public Task<EveNavigation> Navigation { get; }
    }

    public class EveResources : IEveResources
    {
        private readonly IEveResourcesProgress progress;

        private EveData? data = null;
        private readonly TaskSafeMutex dataMutex = new();

        private EveUniverse? universe = null;
        private readonly TaskSafeMutex universeMutex = new();

        private EveNavigation? navigation = null;
        private readonly TaskSafeMutex navigationMutex = new();

        public Task<EveData> Data => GetDataAsync();
        public Task<EveUniverse> Universe => GetUniverseAsync();
        public Task<EveNavigation> Navigation => GetNavigationAsync();

        public EveResources(IEveResourcesProgress progress)
        {
            this.progress = progress;
        }

        private Task<EveData> GetDataAsync()
            => Tasks.Start(GetData);

        private EveData GetData()
        {
            using Locker locker = Locker.Lock(dataMutex);

            if (data is null)
            {
                EmbeddedResource resource = EmbeddedResources.Find("EveData");

                progress.DataProgress.Report(WorkState.Working);
                data = resource.Read(s => new EveData(s), true);
                progress.DataProgress.Report(WorkState.Done);
            }

            return data;
        }

        private Task<EveUniverse> GetUniverseAsync()
            => Tasks.Start(GetUniverse);

        private EveUniverse GetUniverse()
        {
            using Locker locker = Locker.Lock(universeMutex);

            if (universe is null)
            {
                EmbeddedResource resource = EmbeddedResources.Find("EveUniverse");
                EveData eveData = Tasks.Wait(Data);

                progress.UniverseProgress.Report(WorkState.Working);
                universe = resource.Read(s => new EveUniverse(s, eveData), true);
                progress.UniverseProgress.Report(WorkState.Done);
            }

            return universe;
        }

        private Task<EveNavigation> GetNavigationAsync()
            => Tasks.Start(GetNavigation);

        private EveNavigation GetNavigation()
        {
            using Locker locker = Locker.Lock(navigationMutex);

            if (navigation is null)
            {
                EmbeddedResource resource = EmbeddedResources.Find("EveNavigation");
                EveSolarSystems solarSystems = Tasks.Wait(Universe).SolarSystems;

                progress.NavigationProgress.Report(WorkState.Working);
                navigation = resource.Read(s => new EveNavigation(s, solarSystems), true);
                progress.NavigationProgress.Report(WorkState.Done);
            }

            return navigation;
        }
    }

    public interface IDataProvider
    {
        public DateTime Version { get; }
        public EveTypes Types { get; }
        public EveStationOperations StationOperations { get; }
        public EveNpcCorporations NpcCorporations { get; }
        public EvePlanetSchematics PlanetSchematics { get; }
        public EveBlueprints Blueprints { get; }
    }

    public class DataProvider : IDataProvider
    {
        private readonly EveData data;

        public DateTime Version => data.Version;
        public EveTypes Types => data.Types;
        public EveStationOperations StationOperations => data.StationOperations;
        public EveNpcCorporations NpcCorporations => data.NpcCorporations;
        public EvePlanetSchematics PlanetSchematics => data.PlanetSchematics;
        public EveBlueprints Blueprints => data.Blueprints;

        public DataProvider(IEveResources resources)
        {
            data = Tasks.Wait(resources.Data);
        }
    }

    public interface IUniverseProvider
    {
        public EveRegions Regions { get; }
        public EveConstellations Constellations { get; }
        public EveSolarSystems SolarSystems { get; }
        public EveAllPlanets Planets { get; }
        public EveAllMoons Moons { get; }
        public EveStations Stations { get; }
        public EveStargates Stargates { get; }
    }

    public class UniverseProvider : IUniverseProvider
    {
        private readonly EveUniverse universe;

        public EveRegions Regions => universe.Regions;
        public EveConstellations Constellations => universe.Constellations;
        public EveSolarSystems SolarSystems => universe.SolarSystems;
        public EveAllPlanets Planets => universe.Planets;
        public EveAllMoons Moons => universe.Moons;
        public EveStations Stations => universe.Stations;
        public EveStargates Stargates => universe.Stargates;

        public UniverseProvider(IEveResources resources)
        {
            universe = Tasks.Wait(resources.Universe);
        }
    }

    public interface INavigationProvider
    {
        public EveNavigation Navigation { get; }
    }

    public class NavigationProvider : INavigationProvider
    {
        public EveNavigation Navigation { get; }

        public NavigationProvider(IEveResources resources)
        {
            Navigation = Tasks.Wait(resources.Navigation);
        }
    }
}
