using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Services;
using Fasciculus.Threading;
using System.Reflection;

namespace Fasciculus.Eve.PageModels
{
    public partial class InfoPageModel : ObservableObject
    {
        [ObservableProperty]
        private string applicationVersion = "0";

        [ObservableProperty]
        private string sdeVersion = "0";

        [ObservableProperty]
        private SideBarModel sideBar;

        public InfoPageModel(IEveResources resources, SideBarModel sideBar)
        {
            ApplicationVersion = Assembly.GetEntryAssembly().GetVersion();
            SdeVersion = Tasks.Wait(resources.Data).Version.ToString("yyyy-MM-dd");

            SideBar = sideBar;
        }
    }
}
