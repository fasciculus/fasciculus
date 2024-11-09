﻿using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class SdeMoon
    {
        public int TypeId { get; set; }
    }

    public class SdeAsteroidBelt
    {
        public int TypeId { get; set; }
    }

    public class SdePlanet
    {
        public int TypeId { get; set; }
        public Dictionary<int, SdeAsteroidBelt> AsteroidBelts { get; set; } = [];
        public Dictionary<int, SdeMoon> Moons { get; set; } = [];
    }

    public class SdeStargate
    {
        public int Destination { get; set; }
    }

    public class SdeSolarSystem
    {
        public int SolarSystemID { get; set; }
        public double Security { get; set; }
        public string SecurityClass { get; set; } = string.Empty;
        public Dictionary<int, SdeStargate> Stargates { get; set; } = [];
        public Dictionary<int, SdePlanet> Planets { get; set; } = [];

        public string Name { get; set; } = string.Empty;

        public void Populate(SdeData data)
        {
            Name = data.Names[SolarSystemID];
        }
    }

    public class SdeConstellation
    {
        public int ConstellationID { get; set; }
        public SdeSolarSystem[] SolarSystems { get; set; } = [];

        public string Name { get; set; } = string.Empty;

        public void Populate(SdeData data)
        {
            Name = data.Names[ConstellationID];

            SolarSystems.Apply(s => { s.Populate(data); });
        }
    }

    public class SdeRegion
    {
        public int RegionID { get; set; }
        public SdeConstellation[] Constellations { get; set; } = [];

        public string Name { get; set; } = string.Empty;

        public void Populate(SdeData data)
        {
            Name = data.Names[RegionID];

            Constellations.Apply(c => c.Populate(data));
        }
    }

    public class SdeUniverse
    {
        public List<SdeRegion> Regions { get; } = [];

        public SdeUniverse(IEnumerable<SdeRegion> regions)
        {
            regions.Apply(Regions.Add);
        }

        public void Populate(SdeData data)
        {
            Regions.Apply(region => { region.Populate(data); });
        }
    }
}
