namespace Fasciculus.Eve.Assets.Models
{
    public class SdeStargate
    {
        public int Destination { get; set; }
    }

    public class SdeNpcStation
    {
    }

    public static class SdeMoonIndex
    {
        public static readonly AsyncLocal<int> CelestialIndex = new();
    }

    public class SdeMoon
    {
        public Dictionary<int, SdeNpcStation> NpcStations { get; set; } = [];

        public int CelestialIndex { get; }

        public SdeMoon()
        {
            CelestialIndex = SdeMoonIndex.CelestialIndex.Value++;
        }
    }

    public class SdePlanet
    {
        public int CelestialIndex { get; set; }

        public Dictionary<int, SdeMoon> Moons { get; set; } = [];

        public SdePlanet()
        {
            SdeMoonIndex.CelestialIndex.Value = 1;
        }
    }

    public class SdeSolarSystem
    {
        public int SolarSystemID { get; set; }
        public double Security { get; set; }

        public Dictionary<int, SdePlanet> Planets { get; set; } = [];
        public Dictionary<int, SdeStargate> Stargates { get; set; } = [];
    }

    public class SdeConstellation
    {
        public int ConstellationID { get; set; }

        public SdeSolarSystem[] SolarSystems { get; set; } = [];
    }

    public class SdeRegion
    {
        public int RegionID { get; set; }
        public SdeConstellation[] Constellations { get; set; } = [];
    }
}
