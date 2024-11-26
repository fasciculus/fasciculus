namespace Fasciculus.Eve.Assets.Models
{
    public class SdeNpcStation
    {
    }

    public class SdeMoon
    {
        public Dictionary<long, SdeNpcStation> NpcStations { get; set; } = [];
    }

    public class SdePlanet
    {
        public Dictionary<long, SdeMoon> Moons { get; set; } = [];
    }

    public class SdeSolarSystem
    {
        public Dictionary<long, SdePlanet> Planets { get; set; } = [];
    }

    public class SdeConstellation
    {
        public SdeSolarSystem[] SolarSystems { get; set; } = [];
    }

    public class SdeRegion
    {
        public SdeConstellation[] Constellations { get; set; } = [];
    }
}
