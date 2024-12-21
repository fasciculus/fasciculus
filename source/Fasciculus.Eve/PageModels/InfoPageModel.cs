using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Services;
using NuGet.Versioning;

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
            ApplicationVersion = SemanticVersion.Parse(Assembly.GetEntryAssembly().GetSemanticVersion()).ToString();
            SdeVersion = provider.Version.ToString("yyyy-MM-dd");
        }
    }
}
