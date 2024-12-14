using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Services;
using Fasciculus.Eve.Support;
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

        [ObservableProperty]
        private List<string> haulers;

        [ObservableProperty]
        private int selectedHauler;

        public IndustryPageModel(IEveSettings settings, IEveResources resources)
        {
            this.settings = settings.IndustrySettings;
            this.settings.PropertyChanged += OnSettingsChanged;

            solarSystems = [.. Tasks.Wait(resources.Universe).SolarSystems.Select(x => x.Name).OrderBy(x => x)];
            selectedSolarSystem = this.settings.SolarSystem;

            haulers = [.. EveHaulers.Values.Select(x => x.Caption())];
            selectedHauler = GetHaulerIndex(this.settings.MaxVolume);
        }

        private static int GetHaulerIndex(int volume)
            => EveHaulers.Parse(volume).Index();

        private void OnSettingsChanged(object? sender, PropertyChangedEventArgs ev)
        {
            SelectedSolarSystem = settings.SolarSystem;
            SelectedHauler = GetHaulerIndex(settings.MaxVolume);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs ev)
        {
            base.OnPropertyChanged(ev);

            switch (ev.PropertyName)
            {
                case nameof(SelectedSolarSystem):
                    settings.SolarSystem = SelectedSolarSystem;
                    break;

                case nameof(SelectedHauler):
                    settings.MaxVolume = EveHaulers.Values[SelectedHauler].Volume();
                    break;
            }
        }
    }
}
