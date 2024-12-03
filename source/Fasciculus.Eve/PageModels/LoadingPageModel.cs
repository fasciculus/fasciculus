using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Services;
using Fasciculus.Threading;

namespace Fasciculus.Eve.PageModels
{
    public partial class LoadingPageModel : ObservableObject
    {
        private readonly INavigator navigator;

        public LoadingPageModel(INavigator navigator)
        {
            this.navigator = navigator;
        }

        public void OnLoaded()
        {
            Tasks.LongRunning(LoadResources)
                .ContinueWith(GoToMainPage);
        }

        private void LoadResources()
        {
            Tasks.Wait(Task.Delay(2000));
        }

        private Task GoToMainPage(object? _)
        {
            return navigator.GoTo("//Info");
        }
    }
}
