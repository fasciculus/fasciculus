using Fasciculus.Algorithms;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveSecurity
    {
        public enum Level : int
        {
            All = 0,
            LowAndHigh = 1,
            High = 2
        }

        public static Level[] Levels => [Level.All, Level.LowAndHigh, Level.High];

        public delegate bool Filter(double security);

        private static readonly Filter AllFilter = (_) => true;
        private static readonly Filter LowAndHighFilter = (security) => security >= 0.0;
        private static readonly Filter HighFilter = (security) => security >= 0.5;

        public static readonly IReadOnlyDictionary<Level, Filter> Filters = new Dictionary<Level, Filter>()
        {
            { Level.All, AllFilter },
            { Level.LowAndHigh, LowAndHighFilter },
            { Level.High, HighFilter }
        };
    }

    public class EveMoonStation
    {
        public class Data
        {
            public int Id { get; set; }
            public int Operation { get; set; }
            public int Owner { get; set; }

            public Data(int id, int operation, int owner)
            {
                Id = id;
                Operation = operation;
                Owner = owner;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Operation = stream.ReadInt();
                Owner = stream.ReadInt();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteInt(Operation);
                stream.WriteInt(Owner);
            }
        }

        private readonly Data data;

        public int Id => data.Id;

        public EveMoonStation(Data data)
        {
            this.data = data;
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EveMoonStations : IEnumerable<EveMoonStation>
    {
        private readonly Dictionary<int, EveMoonStation> byId;

        public int Count => byId.Count;

        public EveMoonStations(IEnumerable<EveMoonStation> stations)
        {
            byId = stations.ToDictionary(x => x.Id);
        }

        public IEnumerator<EveMoonStation> GetEnumerator()
            => byId.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => byId.Values.GetEnumerator();
    }

    [DebuggerDisplay("{Name}")]
    public class EveMoon
    {
        public class Data
        {
            public int Id { get; }
            public int CelestialIndex { get; }

            public EveMoonStation.Data[] Stations { get; }

            public Data(int id, int celestialIndex, EveMoonStation.Data[] stations)
            {
                Id = id;
                CelestialIndex = celestialIndex;
                Stations = stations;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                CelestialIndex = stream.ReadInt();
                Stations = stream.ReadArray(s => new EveMoonStation.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteInt(CelestialIndex);
                stream.WriteArray(Stations, x => x.Write(stream));
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public int CelestialIndex => data.CelestialIndex;

        public string Name { get; }

        public EveMoonStations Stations { get; }

        public EveMoon(Data data, EvePlanet planet)
        {
            this.data = data;

            Name = $"{planet.Name} - Moon {CelestialIndex}";

            Stations = new(data.Stations.Select(d => new EveMoonStation(d)));
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EveAllMoons : IEnumerable<EveMoon>
    {
        private readonly Dictionary<int, EveMoon> byId;
        private readonly Dictionary<string, EveMoon> byName;

        public int Count => byId.Count;

        public EveMoon this[int id] => byId[id];
        public EveMoon this[string name] => byName[name];

        public EveAllMoons(IEnumerable<EveMoon> moons)
        {
            byId = moons.ToDictionary(x => x.Id);
            byName = moons.ToDictionary(x => x.Name);
        }

        public IEnumerator<EveMoon> GetEnumerator()
            => byId.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => byId.Values.GetEnumerator();
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EvePlanetMoons : IEnumerable<EveMoon>
    {
        private readonly Dictionary<int, EveMoon> byId;
        private readonly Dictionary<int, EveMoon> byIndex;
        private readonly Dictionary<string, EveMoon> byName;

        public int Count => byId.Count;

        public EveMoon this[int idOrIndex] => GetMoon(idOrIndex);
        public EveMoon this[string name] => byName[name];

        public EvePlanetMoons(IEnumerable<EveMoon> moons)
        {
            byId = moons.ToDictionary(x => x.Id);
            byIndex = moons.ToDictionary(x => x.CelestialIndex);
            byName = moons.ToDictionary(x => x.Name);
        }

        private EveMoon GetMoon(int idOrIndex)
        {
            if (byIndex.TryGetValue(idOrIndex, out EveMoon? moon))
            {
                return moon;
            }

            return byId[idOrIndex];
        }

        public IEnumerator<EveMoon> GetEnumerator()
            => byId.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => byId.Values.GetEnumerator();
    }

    [DebuggerDisplay("{Name}")]
    public class EvePlanet
    {
        public class Data
        {
            public int Id { get; }
            public int CelestialIndex;

            public EveMoon.Data[] Moons { get; }

            public Data(int id, int celestialIndex, EveMoon.Data[] moons)
            {
                Id = id;
                CelestialIndex = celestialIndex;
                Moons = moons;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                CelestialIndex = stream.ReadInt();
                Moons = stream.ReadArray(s => new EveMoon.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteInt(CelestialIndex);
                stream.WriteArray(Moons, m => m.Write(stream));
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public int CelestialIndex => data.CelestialIndex;

        public string Name { get; }

        public EvePlanetMoons Moons { get; }

        public EvePlanet(Data data, EveSolarSystem solarSystem)
        {
            this.data = data;

            Name = $"{solarSystem.Name} {RomanNumbers.Format(CelestialIndex)}";

            Moons = new(data.Moons.Select(d => new EveMoon(d, this)));
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EveAllPlanets : IEnumerable<EvePlanet>
    {
        private readonly Dictionary<int, EvePlanet> byId;
        private readonly Dictionary<string, EvePlanet> byName;

        public int Count => byId.Count;

        public EvePlanet this[int id] => byId[id];
        public EvePlanet this[string name] => byName[name];

        public EveAllPlanets(IEnumerable<EvePlanet> planets)
        {
            byId = planets.ToDictionary(p => p.Id);
            byName = planets.ToDictionary(p => p.Name);
        }

        public IEnumerator<EvePlanet> GetEnumerator()
            => byId.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => byId.Values.GetEnumerator();
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EveSystemPlanets : IEnumerable<EvePlanet>
    {
        private readonly Dictionary<int, EvePlanet> byId;
        private readonly Dictionary<int, EvePlanet> byIndex;
        private readonly Dictionary<string, EvePlanet> byName;

        public int Count => byId.Count;

        public EvePlanet this[int idOrIndex] => GetPlanet(idOrIndex);
        public EvePlanet this[string name] => byName[name];

        public EveSystemPlanets(IEnumerable<EvePlanet> planets)
        {
            byId = planets.ToDictionary(p => p.Id);
            byIndex = planets.ToDictionary(p => p.CelestialIndex);
            byName = planets.ToDictionary(p => p.Name);
        }

        private EvePlanet GetPlanet(int idOrIndex)
        {
            if (byIndex.TryGetValue(idOrIndex, out EvePlanet? planet))
            {
                return planet;
            }

            return byId[idOrIndex];
        }

        public IEnumerator<EvePlanet> GetEnumerator()
            => byId.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => byId.Values.GetEnumerator();
    }

    public class EveStargate
    {
        public class Data
        {
            public int Id { get; }
            public int Destination { get; }

            public Data(int id, int destination)
            {
                Id = id;
                Destination = destination;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Destination = stream.ReadInt();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteInt(Destination);
            }
        }

        private readonly Data data;

        public int Id => data.Id;

        public EveStargate(Data data)
        {
            this.data = data;
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EveStargates : IEnumerable<EveStargate>
    {
        private readonly Dictionary<int, EveStargate> byId;

        public int Count => byId.Count;

        public EveStargate this[int id] => byId[id];

        public EveStargates(IEnumerable<EveStargate> stargates)
        {
            byId = stargates.ToDictionary(s => s.Id);
        }

        public IEnumerator<EveStargate> GetEnumerator()
            => byId.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => byId.Values.GetEnumerator();
    }

    [DebuggerDisplay("{Name}")]
    public class EveSolarSystem
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; }
            public double Security { get; }

            public EvePlanet.Data[] Planets { get; }
            public EveStargate.Data[] Stargates { get; }

            public Data(int id, string name, double security, EvePlanet.Data[] planets, EveStargate.Data[] stargates)
            {
                Id = id;
                Name = name;
                Security = security;
                Planets = planets;
                Stargates = stargates;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Name = stream.ReadString();
                Security = stream.ReadDouble();
                Planets = stream.ReadArray(s => new EvePlanet.Data(s));
                Stargates = stream.ReadArray(s => new EveStargate.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteString(Name);
                stream.WriteDouble(Security);
                stream.WriteArray(Planets, p => p.Write(stream));
                stream.WriteArray(Stargates, sg => sg.Write(stream));
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public string Name => data.Name;

        public EveSystemPlanets Planets { get; }
        public EveStargates Stargates { get; }

        public EveSolarSystem(Data data)
        {
            this.data = data;

            Planets = new(data.Planets.Select(d => new EvePlanet(d, this)));
            Stargates = new(data.Stargates.Select(d => new EveStargate(d)));
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EveSolarSystems : IEnumerable<EveSolarSystem>
    {
        private readonly Dictionary<int, EveSolarSystem> byId;
        private readonly Dictionary<string, EveSolarSystem> byName;

        public int Count => byId.Count;

        public EveSolarSystem this[int id] => byId[id];
        public EveSolarSystem this[string name] => byName[name];

        public EveSolarSystems(IEnumerable<EveSolarSystem> solarSystems)
        {
            byId = solarSystems.ToDictionary(s => s.Id);
            byName = solarSystems.ToDictionary(s => s.Name);
        }

        public IEnumerator<EveSolarSystem> GetEnumerator()
            => byId.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => byId.Values.GetEnumerator();
    }

    [DebuggerDisplay("{Name}")]
    public class EveConstellation
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; }

            public EveSolarSystem.Data[] SolarSystems { get; }

            public Data(int id, string name, EveSolarSystem.Data[] solarSystems)
            {
                Id = id;
                Name = name;
                SolarSystems = solarSystems;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Name = stream.ReadString();
                SolarSystems = stream.ReadArray(s => new EveSolarSystem.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteString(Name);
                stream.WriteArray(SolarSystems, s => s.Write(stream));
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public string Name => data.Name;

        public EveSolarSystems SolarSystems { get; }

        public EveConstellation(Data data)
        {
            this.data = data;

            SolarSystems = new(data.SolarSystems.Select(d => new EveSolarSystem(d)));
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EveConstellations : IEnumerable<EveConstellation>
    {
        private readonly Dictionary<int, EveConstellation> byId;
        private readonly Dictionary<string, EveConstellation> byName;

        public int Count => byId.Count;

        public EveConstellation this[int id] => byId[id];
        public EveConstellation this[string name] => byName[name];

        public EveConstellations(IEnumerable<EveConstellation> constellations)
        {
            byId = constellations.ToDictionary(c => c.Id);
            byName = constellations.ToDictionary(c => c.Name);
        }

        public IEnumerator<EveConstellation> GetEnumerator()
            => byId.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => byId.Values.GetEnumerator();
    }

    [DebuggerDisplay("{Name}")]
    public class EveRegion
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; }
            public EveConstellation.Data[] Constellations { get; }

            public Data(int id, string name, EveConstellation.Data[] constellations)
            {
                Id = id;
                Name = name;
                Constellations = constellations;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Name = stream.ReadString();
                Constellations = stream.ReadArray(s => new EveConstellation.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteString(Name);
                stream.WriteArray(Constellations, c => c.Write(stream));
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public string Name => data.Name;

        public EveConstellations Constellations { get; }

        public EveRegion(Data data)
        {
            this.data = data;

            Constellations = new(data.Constellations.Select(d => new EveConstellation(d)));
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EveRegions : IEnumerable<EveRegion>
    {
        private readonly Dictionary<int, EveRegion> byId;
        private readonly Dictionary<string, EveRegion> byName;

        public int Count => byId.Count;

        public EveRegion this[int id] => byId[id];
        public EveRegion this[string name] => byName[name];

        public EveRegions(IEnumerable<EveRegion> regions)
        {
            byId = regions.ToDictionary(r => r.Id);
            byName = regions.ToDictionary(r => r.Name);
        }

        public IEnumerator<EveRegion> GetEnumerator()
            => byId.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => byId.Values.GetEnumerator();
    }

    public class EveUniverse
    {
        public class Data
        {
            public EveRegion.Data[] Regions { get; }

            public Data(EveRegion.Data[] regions)
            {
                Regions = regions;
            }

            public Data(Stream stream)
            {
                Regions = stream.ReadArray(s => new EveRegion.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteArray(Regions, r => r.Write(stream));
            }
        }

        private readonly Data data;

        public EveRegions Regions { get; }
        public EveConstellations Constellations { get; }
        public EveSolarSystems SolarSystems { get; }
        public EveAllPlanets Planets { get; }
        public EveAllMoons Moons { get; }
        public EveStargates Stargates { get; }

        public EveUniverse(Data data)
        {
            this.data = data;

            Regions = new(data.Regions.Select(d => new EveRegion(d)));
            Constellations = new(Regions.SelectMany(r => r.Constellations));
            SolarSystems = new(Constellations.SelectMany(c => c.SolarSystems));
            Planets = new(SolarSystems.SelectMany(s => s.Planets));
            Moons = new(Planets.SelectMany(p => p.Moons));
            Stargates = new(SolarSystems.SelectMany(s => s.Stargates));
        }

        public EveUniverse(Stream stream)
            : this(new Data(stream)) { }

        public void Write(Stream stream)
        {
            data.Write(stream);
        }
    }
}
