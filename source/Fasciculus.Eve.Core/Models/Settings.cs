using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Maui.ComponentModel;
using System.Text.Json.Serialization;

namespace Fasciculus.Eve.Models
{
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
        public EvePlanetsSettings Planets { get; set; } = new();
    }
}
