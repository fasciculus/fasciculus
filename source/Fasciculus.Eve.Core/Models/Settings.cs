using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Maui.ComponentModel;
using System.Text.Json.Serialization;

namespace Fasciculus.Eve.Models
{
    public partial class EvePlanetsSettings : MainThreadObservable
    {
        [JsonIgnore]
        [ObservableProperty]
        private double customsTaxRate = 0.10;

        [JsonIgnore]
        [ObservableProperty]
        private double sellTaxRate = 0.03;
    }

    public partial class EveCombinedSettings
    {
        public EvePlanetsSettings Planets { get; set; } = new();
    }

    [JsonSerializable(typeof(EveCombinedSettings))]
    [JsonSourceGenerationOptions(WriteIndented = true)]
    public partial class EveSettingsContext : JsonSerializerContext
    {
    }
}
