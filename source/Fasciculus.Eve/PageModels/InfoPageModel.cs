using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Services;
using System.Reflection;

namespace Fasciculus.Eve.PageModels
{
    public partial class InfoPageModel : ObservableObject
    {
        [ObservableProperty]
        public partial string ApplicationVersion { get; private set; }

        [ObservableProperty]
        public partial string SdeVersion { get; private set; }

        public InfoPageModel(IEveProvider provider)
        {
            ApplicationVersion = Assembly.GetEntryAssembly().GetVersion();
            SdeVersion = provider.Version.ToString("yyyy-MM-dd");
        }
    }
}
