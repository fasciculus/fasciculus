using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Services;
using Fasciculus.Maui.Support.Progressing;
using Fasciculus.Threading;

namespace Fasciculus.Eve.PageModels
{
    public partial class LoadingPageModel : MainThreadObservable
    {
        private readonly IEveResources resources;
        private readonly INavigator navigator;

        [ObservableProperty]
        public partial string State { get; private set; }

        public ProgressBarProgress Progress { get; }

        public LoadingPageModel(IEveResources resources, INavigator navigator)
        {
            this.resources = resources;
            this.navigator = navigator;

            State = string.Empty;
            Progress = new();
        }

        public void OnLoaded()
        {
            Tasks.LongRunning(LoadResources)
                .ContinueWith(_ => navigator.GoTo("//Info").WaitFor());
        }

        private void LoadResources()
        {
            State = "Common Data";
            Progress.Begin(4);
            Progress.Report(1);
            Tasks.Wait(resources.Data);
            Tasks.Sleep(250);

            State = "Universe";
            Progress.Report(1);
            Tasks.Wait(resources.Universe);
            Tasks.Sleep(250);

            State = "Navigation";
            Progress.Report(1);
            Tasks.Wait(resources.Navigation);
            Tasks.Sleep(250);

            State = "Done";
            Progress.End();
            Tasks.Sleep(500);
        }
    }
}
