namespace Fasciculus.Eve.Assets.Models
{
    public class SdeStargate
    {
        public uint Destination { get; set; }
    }

    public class SdeMoonStation
    {
        public int OperationID { get; set; }
        public int OwnerID { get; set; }
    }

    public static class SdeMoonIndex
    {
        public static readonly AsyncLocal<int> CelestialIndex = new();
    }

    public class SdeMoon
    {
        public Dictionary<int, SdeMoonStation> NpcStations { get; set; } = [];

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
        public uint SolarSystemID { get; set; }
        public double Security { get; set; }

        public Dictionary<int, SdePlanet> Planets { get; set; } = [];
        public Dictionary<uint, SdeStargate> Stargates { get; set; } = [];
    }

    public class SdeConstellation
    {
        public uint ConstellationID { get; set; }

        public SdeSolarSystem[] SolarSystems { get; set; } = [];
    }

    public class SdeRegion
    {
        public uint RegionID { get; set; }
        public SdeConstellation[] Constellations { get; set; } = [];
    }
}
