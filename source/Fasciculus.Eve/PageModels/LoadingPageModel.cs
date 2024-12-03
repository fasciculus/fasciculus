using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.Services;
using Fasciculus.Threading;

namespace Fasciculus.Eve.PageModels
{
    public partial class LoadingPageModel : ObservableObject
    {
        private readonly IEveResources resources;
        private readonly INavigator navigator;

        public LoadingPageModel(IEveResources resources, INavigator navigator)
        {
            this.resources = resources;
            this.navigator = navigator;
        }

        public void OnLoaded()
        {
            Tasks.LongRunning(LoadResources)
                .ContinueWith(GoToMainPage);
        }

        private void LoadResources()
        {
            EveData _ = resources.Data;
        }

        private Task GoToMainPage(object? _)
        {
            return navigator.GoTo("//Info");
        }
    }
}
