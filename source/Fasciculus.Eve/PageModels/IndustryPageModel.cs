using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Threading;
using System.ComponentModel;

namespace Fasciculus.Eve.PageModels
{
    public partial class IndustryPageModel : MainThreadObservable
    {
        private readonly EveIndustrySettings settings;

        [ObservableProperty]
        private List<string> solarSystems;

        [ObservableProperty]
        private string selectedSolarSystem;

        public IndustryPageModel(IEveSettings settings, IEveResources resources)
        {
            this.settings = settings.IndustrySettings;
            this.settings.PropertyChanged += OnSettingsChanged;

            solarSystems = [.. Tasks.Wait(resources.Universe).SolarSystems.Select(x => x.Name).OrderBy(x => x)];
            selectedSolarSystem = this.settings.SolarSystem;
        }

        private void OnSettingsChanged(object? sender, PropertyChangedEventArgs ev)
        {
            SelectedSolarSystem = settings.SolarSystem;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs ev)
        {
            base.OnPropertyChanged(ev);

            if (ev.PropertyName == nameof(SelectedSolarSystem))
            {
                settings.SolarSystem = SelectedSolarSystem;
            }
        }
    }
}
