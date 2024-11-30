using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Pages.Controls;

namespace Fasciculus.Eve.PageModels
{
    public partial class IndustryPageModel : ObservableObject
    {
        [ObservableProperty]
        private SideBarModel sideBar;

        public IndustryPageModel(SideBarModel sideBar)
        {
            SideBar = sideBar;
        }
    }
}
