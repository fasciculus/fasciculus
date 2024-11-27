using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Pages.Controls;

namespace Fasciculus.Eve.PageModels
{
    public partial class MarketPageModel : ObservableObject
    {
        [ObservableProperty]
        private SideBarModel sideBar;

        public MarketPageModel(SideBarModel sideBar)
        {
            SideBar = sideBar;
        }
    }
}
