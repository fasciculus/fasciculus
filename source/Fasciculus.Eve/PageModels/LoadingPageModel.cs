using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Services;
using Fasciculus.Maui.Support;
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
        private WorkState data = WorkState.Pending;

        [ObservableProperty]
        private WorkState universe = WorkState.Pending;

        [ObservableProperty]
        private WorkState navigation = WorkState.Pending;

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
            Tasks.Wait(resources.Data);
            Tasks.Wait(resources.Universe);
            Tasks.Wait(resources.Navigation);
        }

        private void OnProgressChanged(object? sender, PropertyChangedEventArgs ev)
        {
            Data = progress.DataInfo;
            Universe = progress.UniverseInfo;
            Navigation = progress.NavigationInfo;
        }

        private Task GoToMainPage(object? _)
        {
            return navigator.GoTo("//Info");
        }
    }
}
