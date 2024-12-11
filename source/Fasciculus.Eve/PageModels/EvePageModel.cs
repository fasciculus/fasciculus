using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Maui.Support;

namespace Fasciculus.Eve.PageModels
{
    public partial class EvePageModel : MainThreadObservable
    {
        [ObservableProperty]
        private SideBarModel sideBar;

        [ObservableProperty]
        private StatusBarModel statusBar;

        public EvePageModel()
        {
            sideBar = MauiAppServices.GetRequiredService<SideBarModel>();
            statusBar = MauiAppServices.GetRequiredService<StatusBarModel>();
        }
    }
}
