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

        public InfoPageModel(IEveProvider provider)
        {
            applicationVersion = Assembly.GetEntryAssembly().GetVersion();
            sdeVersion = provider.Version.ToString("yyyy-MM-dd");
        }
    }
}
