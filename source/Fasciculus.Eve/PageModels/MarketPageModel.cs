using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Services;
using Fasciculus.Threading;

namespace Fasciculus.Eve.PageModels
{
    public partial class MarketPageModel : ObservableObject
    {
        [ObservableProperty]
        private SideBarModel sideBar;

        [ObservableProperty]
        private string hub;

        public MarketPageModel(SideBarModel sideBar, IEveResources resources)
        {
            SideBar = sideBar;

            hub = Tasks.Wait(resources.Universe).NpcStations[60003760].FullName;
        }
    }
}
