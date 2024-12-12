using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Support;
using System.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class PlanetsPageModel : MainThreadObservable
    {
        private readonly IPlanets planets;

        [ObservableProperty]
        private string hub;

        [ObservableProperty]
        private LongProgressInfo buyProgress = LongProgressInfo.Start;

        [ObservableProperty]
        private LongProgressInfo sellProgress = LongProgressInfo.Start;

        [ObservableProperty]
        private EvePlanetProduction[] productions = [];

        public PlanetsPageModel(IPlanets planets)
        {
            this.planets = planets;
            this.planets.PropertyChanged += OnPlanetsChanged;

            hub = planets.Hub.FullName;
        }

        private void OnPlanetsChanged(object? sender, PropertyChangedEventArgs ev)
        {
            SellProgress = planets.SellProgressInfo;
            BuyProgress = planets.BuyProgressInfo;
            Productions = planets.Productions;
        }

        [RelayCommand]
        private Task Start()
        {
            return planets.StartAsync();
        }
    }
}
