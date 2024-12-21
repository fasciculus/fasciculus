using CommunityToolkit.Mvvm.ComponentModel;
using Fasciculus.Maui.ComponentModel;
using System.Text.Json.Serialization;

namespace Fasciculus.Eve.Models
{
    public partial class EveIndustrySettings : MainThreadObservable
    {
        [field: JsonIgnore]
        [ObservableProperty]
        public partial string SolarSystem { get; set; }

        [field: JsonIgnore]
        [ObservableProperty]
        public partial int MaxVolume { get; set; }

        [field: JsonIgnore]
        [ObservableProperty]
        public partial bool IgnoreSkills { get; set; }

        [field: JsonIgnore]
        [ObservableProperty]
        public partial bool IncludeT2 { get; set; }

        [field: JsonIgnore]
        [ObservableProperty]
        public partial int SalesTaxRate { get; set; }

        public EveIndustrySettings()
        {
            SolarSystem = "Jita";
            MaxVolume = 0;
            IgnoreSkills = false;
            SalesTaxRate = 30;
        }
    }

    public partial class EvePlanetsSettings : MainThreadObservable
    {
        [field: JsonIgnore]
        [ObservableProperty]
        public partial int CustomsTaxRate { get; set; }

        [field: JsonIgnore]
        [ObservableProperty]
        public partial int SalesTaxRate { get; set; }

        [field: JsonIgnore]
        [ObservableProperty]
        public partial int ProductionLines { get; set; }

        [field: JsonIgnore]
        [ObservableProperty]
        public partial int HoursPerDay { get; set; }

        public EvePlanetsSettings()
        {
            CustomsTaxRate = 30;
            SalesTaxRate = 30;
            ProductionLines = 1;
            HoursPerDay = 22;
        }
    }

    public partial class EveCombinedSettings
    {
        public EveIndustrySettings Industry { get; set; }
        public EvePlanetsSettings Planets { get; set; }

        public EveCombinedSettings()
        {
            Industry = new();
            Planets = new();
        }
    }
}
