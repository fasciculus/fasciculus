using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Services;

namespace Fasciculus.Eve.PageModels
{
    public partial class PlanetaryIndustryPageModel : EvePageModel
    {
        private readonly IPlanetaryIndustry planetaryIndustry;

        public PlanetaryIndustryPageModel(IPlanetaryIndustry planetaryIndustry)
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
