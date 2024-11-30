using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.IO;
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

        public InfoPageModel(IEmbeddedResources resources, SideBarModel sideBar)
        {
            DateTime sdeVersion = DateTime.FromBinary(resources["SdeVersion"].Read(s => s.ReadLong(), false));

            ApplicationVersion = Assembly.GetEntryAssembly().GetVersion();
            SdeVersion = sdeVersion.ToString("yyyy-MM-dd");

            SideBar = sideBar;
        }
    }
}
