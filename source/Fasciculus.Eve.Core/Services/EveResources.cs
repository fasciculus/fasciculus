using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.IO;
using Fasciculus.Maui.Support;
using Fasciculus.Threading;
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
        private WorkState dataInfo;
        public IProgress<WorkState> DataProgress { get; }

        [ObservableProperty]
        private WorkState universeInfo;
        public IProgress<WorkState> UniverseProgress { get; }

        [ObservableProperty]
        private WorkState navigationInfo;
        public IProgress<WorkState> NavigationProgress { get; }

        [ObservableProperty]
        private bool universe;

        [ObservableProperty]
        private bool navigation;

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
        private readonly IEmbeddedResources resources;
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

        public EveResources(IEmbeddedResources resources, IEveResourcesProgress progress)
        {
            this.resources = resources;
            this.progress = progress;
        }

        private Task<EveData> GetDataAsync()
            => Tasks.Start(GetData);

        private EveData GetData()
        {
            using Locker locker = Locker.Lock(dataMutex);

            if (data is null)
            {
                IEmbeddedResource resource = resources["EveData"];

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
                IEmbeddedResource resource = resources["EveUniverse"];
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
                IEmbeddedResource resource = resources["EveNavigation"];
                EveSolarSystems solarSystems = Tasks.Wait(Universe).SolarSystems;

                progress.NavigationProgress.Report(WorkState.Working);
                navigation = resource.Read(s => new EveNavigation(s, solarSystems), true);
                progress.NavigationProgress.Report(WorkState.Done);
            }

            return navigation;
        }
    }
}
