using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Services;
using Fasciculus.Maui.Support;
using Fasciculus.Threading;

namespace Fasciculus.Eve.PageModels
{
    public partial class LoadingPageModel : MainThreadObservable
    {
        private readonly IEveResources resources;
        private readonly INavigator navigator;

        [ObservableProperty]
        public partial WorkState Data { get; private set; }

        [ObservableProperty]
        public partial WorkState Universe { get; private set; }

        [ObservableProperty]
        public partial WorkState Navigation { get; private set; }

        public LoadingPageModel(IEveResources resources, INavigator navigator)
        {
            this.resources = resources;
            this.navigator = navigator;
            Data = WorkState.Pending;
            Universe = WorkState.Pending;
            Navigation = WorkState.Pending;
        }

        public void OnLoaded()
        {
            Tasks.LongRunning(LoadResources)
                .ContinueWith(_ => Tasks.Sleep(500))
                .ContinueWith(_ => Tasks.Wait(navigator.GoTo("//Info")));
        }

        private void LoadResources()
        {
            Data = WorkState.Working;
            Tasks.Wait(resources.Data);
            Data = WorkState.Done;

            Universe = WorkState.Working;
            Tasks.Wait(resources.Universe);
            Universe = WorkState.Done;

            Navigation = WorkState.Working;
            Tasks.Wait(resources.Navigation);
            Navigation = WorkState.Done;
        }
    }
}
