using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Services;
using System.Reflection;

namespace Fasciculus.Eve.PageModels
{
    public partial class InfoPageModel : ObservableObject
    {
        [ObservableProperty]
        private string applicationVersion;

        [ObservableProperty]
        private string sdeVersion;

        public InfoPageModel(IDataProvider data)
        {
            applicationVersion = Assembly.GetEntryAssembly().GetVersion();
            sdeVersion = data.Version.ToString("yyyy-MM-dd");
        }
    }
}
