using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Services;
using Fasciculus.Maui.Support;
using Fasciculus.Support;
using Fasciculus.Threading;

namespace Fasciculus.Eve.PageModels
{
    public partial class LoadingPageModel : MainThreadObservable
    {
        private readonly IEveResources resources;
        private readonly INavigator navigator;

        [ObservableProperty]
        private WorkState data = WorkState.Pending;

        [ObservableProperty]
        private WorkState universe = WorkState.Pending;

        [ObservableProperty]
        private WorkState navigation = WorkState.Pending;

        [ObservableProperty]
        private WorkState skills = WorkState.Pending;

        public LoadingPageModel(IEveResources resources, INavigator navigator)
        {
            this.resources = resources;
            this.navigator = navigator;
        }

        public void OnLoaded()
        {
            Tasks.LongRunning(LoadResources)
                .ContinueWith(_ => Tasks.Sleep(500))
                .ContinueWith(_ => Tasks.Wait(GoToInfoPage()));
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

            Skills = WorkState.Working;
            GlobalServices.GetRequiredService<ISkillManager>();
            Skills = WorkState.Done;
        }

        private Task GoToInfoPage()
        {
            return navigator.GoTo("//Info");
        }
    }
}
