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
        private bool hasProductions = false;

        [ObservableProperty]
        private EvePlanetProduction[] productions = [];

        [ObservableProperty]
        private EvePlanetProduction? production = null;

        [ObservableProperty]
        private EvePlanetInput[] inputs = [];

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
            HasProductions = Productions.Length > 0;

            if (HasProductions)
            {
                Production = Productions[0];
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs ev)
        {
            base.OnPropertyChanged(ev);

            if (ev.PropertyName == nameof(Production))
            {
                Inputs = Production is null ? [] : [.. Production.Inputs];
            }
        }

        [RelayCommand]
        private Task Start()
        {
            Production = null;
            return planets.StartAsync();
        }
    }
}
