﻿namespace Fasciculus.Eve.Assets.Models
{
    public class SdeNpcStation
    {
    }

    public static class SdeMoonIndex
    {
        public static ThreadLocal<int> CelestialIndex = new();
    }

    public class SdeMoon
    {
        public Dictionary<long, SdeNpcStation> NpcStations { get; set; } = [];

        public int CelestialIndex { get; set; }

        public SdeMoon()
        {
            CelestialIndex = SdeMoonIndex.CelestialIndex.Value++;
        }
    }

    public class SdePlanet
    {
        public Dictionary<long, SdeMoon> Moons { get; set; } = [];

        public SdePlanet()
        {
            SdeMoonIndex.CelestialIndex.Value = 1;
        }
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