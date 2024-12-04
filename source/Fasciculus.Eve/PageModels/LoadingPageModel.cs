using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Services;
using Fasciculus.Threading;
using System.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class LoadingPageModel : MainThreadObservable
    {
        private readonly IEveResources resources;
        private readonly IEveResourcesProgress progress;
        private readonly INavigator navigator;

        [ObservableProperty]
        private string dataText = "Pending";

        [ObservableProperty]
        private Color dataColor = Colors.Orange;

        [ObservableProperty]
        private string universeText = "Pending";

        [ObservableProperty]
        private Color universeColor = Colors.Orange;

        [ObservableProperty]
        private string navigationText = "Pending";

        [ObservableProperty]
        private Color navigationColor = Colors.Orange;

        public LoadingPageModel(IEveResources resources, IEveResourcesProgress progress, INavigator navigator)
        {
            this.resources = resources;
            this.progress = progress;
            this.navigator = navigator;

            this.progress.PropertyChanged += OnProgressChanged;
        }

        public void OnLoaded()
        {
            Tasks.LongRunning(LoadResources)
                .ContinueWith(_ => Tasks.Wait(Task.Delay(200)))
                .ContinueWith(GoToMainPage);
        }

        private void LoadResources()
        {
            Task<EveData> data = resources.Data;
            Task<EveUniverse> universe = resources.Universe;
            Task<EveNavigation> navigation = resources.Navigation;

            Task.WaitAll([data, universe, navigation]);
        }

        private void OnProgressChanged(object? sender, PropertyChangedEventArgs ev)
        {
            DataText = progress.Data ? "Done" : "Pending";
            DataColor = progress.Data ? Colors.Green : Colors.Orange;

            UniverseText = progress.Universe ? "Done" : "Pending";
            UniverseColor = progress.Universe ? Colors.Green : Colors.Orange;

            NavigationText = progress.Navigation ? "Done" : "Pending";
            NavigationColor = progress.Navigation ? Colors.Green : Colors.Orange;
        }

        private Task GoToMainPage(object? _)
        {
            return navigator.GoTo("//Info");
        }
    }
}
