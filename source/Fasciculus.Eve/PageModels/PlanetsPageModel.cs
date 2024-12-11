using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class PlanetsPageModel : MainThreadObservable
    {
        private readonly IPlanetaryIndustry planetaryIndustry;

        public PlanetsPageModel(IPlanetaryIndustry planetaryIndustry)
        {
            this.planetaryIndustry = planetaryIndustry;
        }

        [RelayCommand]
        private Task Start()
        {
            return planetaryIndustry.StartAsync();
        }
    }
}
