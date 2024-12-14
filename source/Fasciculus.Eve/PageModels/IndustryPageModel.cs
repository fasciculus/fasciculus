using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Services;
using Fasciculus.Threading;

namespace Fasciculus.Eve.PageModels
{
    public partial class IndustryPageModel : ObservableObject
    {
        [ObservableProperty]
        private List<string> solarSystems;

        [ObservableProperty]
        private string selectedSolarSystem;

        public IndustryPageModel(IEveResources resources)
        {
            solarSystems = [.. Tasks.Wait(resources.Universe).SolarSystems.Select(x => x.Name).OrderBy(x => x)];
            selectedSolarSystem = "Jita";
        }
    }
}
