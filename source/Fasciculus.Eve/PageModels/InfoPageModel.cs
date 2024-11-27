using CommunityToolkit.Mvvm.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class InfoPageModel : ObservableObject
    {
        [ObservableProperty]
        private string version = "0.1.x";

        [ObservableProperty]
        private SideBarModel sideBar;

        public InfoPageModel(SideBarModel sideBar)
        {
            SideBar = sideBar;
        }
    }
}
