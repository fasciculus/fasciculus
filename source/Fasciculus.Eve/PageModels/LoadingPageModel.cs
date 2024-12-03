using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Services;
using Fasciculus.Threading;

namespace Fasciculus.Eve.PageModels
{
    public partial class LoadingPageModel : MainThreadObservable
    {
        private readonly IEveResources resources;
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

        public LoadingPageModel(IEveResources resources, INavigator navigator)
        {
            this.resources = resources;
            this.navigator = navigator;
        }

        public void OnLoaded()
        {
            Tasks.LongRunning(LoadResources)
                .ContinueWith(_ => Tasks.Wait(Task.Delay(250)))
                .ContinueWith(GoToMainPage);
        }

        private void LoadResources()
        {
            Task data = resources.Data
                .ContinueWith(_ => { DataText = "Done"; DataColor = Colors.Green; });

            Task universe = resources.Universe
                .ContinueWith(_ => { UniverseText = "Done"; UniverseColor = Colors.Green; });

            Task navigation = resources.Navigation
                .ContinueWith(_ => { NavigationText = "Done"; NavigationColor = Colors.Green; });

            Task.WaitAll([data, universe, navigation]);
        }

        private Task GoToMainPage(object? _)
        {
            return navigator.GoTo("//Info");
        }
    }
}
