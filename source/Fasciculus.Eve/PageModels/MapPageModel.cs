using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Pages.Controls;

namespace Fasciculus.Eve.PageModels
{
    public partial class MapPageModel : ObservableObject
    {
        [ObservableProperty]
        private SideBarModel sideBar;

        public MapPageModel(SideBarModel sideBar)
        {
            SideBar = sideBar;
        }
    }
}
