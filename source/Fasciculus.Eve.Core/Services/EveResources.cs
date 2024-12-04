using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.IO;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Support;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Services
{
    public interface IEveResourcesProgress : INotifyPropertyChanged
    {
        public IProgress<bool> DataProgress { get; }
        public IProgress<bool> UniverseProgress { get; }
        public IProgress<bool> NavigationProgress { get; }

        public bool Data { get; }
        public bool Universe { get; }
        public bool Navigation { get; }
    }

    public partial class EveResourcesProgress : MainThreadObservable, IEveResourcesProgress
    {
        public IProgress<bool> DataProgress { get; }
        public IProgress<bool> UniverseProgress { get; }
        public IProgress<bool> NavigationProgress { get; }

        [ObservableProperty]
        private bool data;

        [ObservableProperty]
        private bool universe;

        [ObservableProperty]
        private bool navigation;

        public EveResourcesProgress()
        {
            DataProgress = new TaskSafeProgress<bool>((done) => { Data = done; });
            UniverseProgress = new TaskSafeProgress<bool>((done) => { Universe = done; });
            NavigationProgress = new TaskSafeProgress<bool>((done) => { Navigation = done; });
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

                progress.DataProgress.Report(false);
                data = resource.Read(s => new EveData(s), true);
                progress.DataProgress.Report(true);
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

                progress.UniverseProgress.Report(false);
                universe = resource.Read(s => new EveUniverse(s, eveData), true);
                progress.UniverseProgress.Report(true);
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

                progress.NavigationProgress.Report(false);
                navigation = resource.Read(s => new EveNavigation(s, solarSystems), true);
                progress.NavigationProgress.Report(true);
            }

            return navigation;
        }
    }

    public static class EveResourcesServices
    {
        public static IServiceCollection AddEveResources(this IServiceCollection services)
        {
            services.AddEmbeddedResources();

            services.TryAddSingleton<IEveResourcesProgress, EveResourcesProgress>();
            services.TryAddSingleton<IEveResources, EveResources>();

            return services;
        }
    }
}
