using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class PlanetsPageModel : MainThreadObservable
    {
        private readonly IPlanets planets;

        public PlanetsPageModel(IPlanets planets)
        {
            this.planets = planets;
        }

        [RelayCommand]
        private Task Start()
        {
            return planets.StartAsync();
        }
    }
}
