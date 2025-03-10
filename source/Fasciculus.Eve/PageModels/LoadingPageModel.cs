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
            Tasks.Start(LoadResources, true)
                .ContinueWith(_ => Tasks.Wait(navigator.GoTo("//Info")));
        }

        private void LoadResources()
        {
            State = "Common Data";
            Progress.Begin(4);
            Progress.Report(1);
            Tasks.Wait(resources.Data);
            Tasks.Wait(Task.Delay(250));

            State = "Universe";
            Progress.Report(1);
            Tasks.Wait(resources.Universe);
            Tasks.Wait(Task.Delay(250));

            State = "Navigation";
            Progress.Report(1);
            Tasks.Wait(resources.Navigation);
            Tasks.Wait(Task.Delay(250));

            State = "Done";
            Progress.End();
            Tasks.Wait(Task.Delay(500));
        }
    }
}
