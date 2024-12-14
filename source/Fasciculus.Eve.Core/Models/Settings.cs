using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Maui.ComponentModel;
using System.Text.Json.Serialization;

namespace Fasciculus.Eve.Models
{
    public partial class EveIndustrySettings : MainThreadObservable
    {
        [JsonIgnore]
        [ObservableProperty]
        private string solarSystem = "Jita";

        [JsonIgnore]
        [ObservableProperty]
        private int maxVolume = 0;
    }

    public partial class EvePlanetsSettings : MainThreadObservable
    {
        [JsonIgnore]
        [ObservableProperty]
        private int customsTaxRate = 100;

        [JsonIgnore]
        [ObservableProperty]
        private int salesTaxRate = 30;
    }

    public partial class EveCombinedSettings
    {
        public EveIndustrySettings Industry { get; set; } = new();
        public EvePlanetsSettings Planets { get; set; } = new();
    }
}
