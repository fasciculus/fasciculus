using Fasciculus.Algorithms;
using System;
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

    [DebuggerDisplay("{FullName}")]
    public class EveMoonStation
    {
        public class Data
        {
            public long Id { get; }
            public int Operation { get; }
            public int Owner { get; }

            public Data(long id, int operation, int owner)
            {
                Id = id;
                Operation = operation;
                Owner = owner;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadLong();
                Operation = stream.ReadInt();
                Owner = stream.ReadInt();
            }

            public void Write(Stream stream)
            {
                stream.WriteLong(Id);
                stream.WriteInt(Operation);
                stream.WriteInt(Owner);
            }
        }

        private readonly Data data;

        public long Id => data.Id;

        public string Name { get; }
        public string FullName { get; }

        public EveMoon Moon { get; }
        public EveStationOperation Operation { get; }
        public EveNpcCorporation Owner { get; }

        public EveMoonStation(Data data, EveMoon moon, EveData eveData)
        {
            this.data = data;

            Moon = moon;
            Operation = eveData.StationOperations[data.Operation];
            Owner = eveData.NpcCorporations[data.Owner];

            Name = $"{Owner.Name} {Operation.Name}";
            FullName = $"{Moon.Name} - {Name}";
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EveMoonStations : IEnumerable<EveMoonStation>
    {
        private readonly EveMoonStation[] stations;

        private readonly Lazy<Dictionary<long, EveMoonStation>> byId;

        public int Count => stations.Length;

        public bool Contains(long id) => byId.Value.ContainsKey(id);
        public EveMoonStation this[long id] => byId.Value[id];

        public EveMoonStations(IEnumerable<EveMoonStation> stations)
        {
            this.stations = stations.ToArray();

            byId = new(() => this.stations.ToDictionary(x => x.Id), true);
        }

        public IEnumerator<EveMoonStation> GetEnumerator()
            => stations.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => stations.GetEnumerator();
    }

    [DebuggerDisplay("{Name}")]
    public class EveMoon
    {
        public class Data
        {
            public int Id { get; }
            public int CelestialIndex { get; }

            private readonly EveMoonStation.Data[] stations;
            public IReadOnlyList<EveMoonStation.Data> Stations => stations;

            public Data(int id, int celestialIndex, IEnumerable<EveMoonStation.Data> stations)
            {
                Id = id;
                CelestialIndex = celestialIndex;
                this.stations = stations.ToArray();
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                CelestialIndex = stream.ReadInt();
                stations = stream.ReadArray(s => new EveMoonStation.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteInt(CelestialIndex);
                stream.WriteArray(stations, x => x.Write(stream));
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public int CelestialIndex => data.CelestialIndex;

        public string Name { get; }

        public EvePlanet Planet { get; }
        public EveMoonStations Stations { get; }

        public EveMoon(Data data, EvePlanet planet, EveData eveData)
        {
            this.data = data;

            Name = $"{planet.Name} - Moon {CelestialIndex}";

            Planet = planet;
            Stations = new(data.Stations.Select(d => new EveMoonStation(d, this, eveData)));
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EveAllMoons : IEnumerable<EveMoon>
    {
        private readonly EveMoon[] moons;

        private readonly Lazy<Dictionary<int, EveMoon>> byId;
        private readonly Lazy<Dictionary<string, EveMoon>> byName;

        public int Count => moons.Length;

        public EveMoon this[int id] => byId.Value[id];
        public EveMoon this[string name] => byName.Value[name];

        public EveAllMoons(IEnumerable<EveMoon> moons)
        {
            this.moons = moons.ToArray();

            byId = new(() => this.moons.ToDictionary(x => x.Id), true);
            byName = new(() => this.moons.ToDictionary(x => x.Name), true);
        }

        public IEnumerator<EveMoon> GetEnumerator()
            => moons.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => moons.GetEnumerator();
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EvePlanetMoons : IEnumerable<EveMoon>
    {
        private readonly EveMoon[] moons;

        private readonly Lazy<Dictionary<int, EveMoon>> byId;
        private readonly Lazy<Dictionary<int, EveMoon>> byIndex;
        private readonly Lazy<Dictionary<string, EveMoon>> byName;

        public int Count => moons.Length;

        public EveMoon this[int idOrIndex] => GetMoon(idOrIndex);
        public EveMoon this[string name] => byName.Value[name];

        public EvePlanetMoons(IEnumerable<EveMoon> moons)
        {
            this.moons = moons.ToArray();

            byId = new(() => this.moons.ToDictionary(x => x.Id), true);
            byIndex = new(() => this.moons.ToDictionary(x => x.CelestialIndex), true);
            byName = new(() => this.moons.ToDictionary(x => x.Name), true);
        }

        private EveMoon GetMoon(int idOrIndex)
        {
            if (byIndex.Value.TryGetValue(idOrIndex, out EveMoon? moon))
            {
                return moon;
            }

            return byId.Value[idOrIndex];
        }

        public IEnumerator<EveMoon> GetEnumerator()
            => moons.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => moons.GetEnumerator();
    }

    [DebuggerDisplay("{Name}")]
    public class EvePlanet
    {
        public class Data
        {
            public int Id { get; }
            public int CelestialIndex;

            private EveMoon.Data[] moons;
            public IReadOnlyList<EveMoon.Data> Moons => moons;

            public Data(int id, int celestialIndex, IEnumerable<EveMoon.Data> moons)
            {
                Id = id;
                CelestialIndex = celestialIndex;
                this.moons = moons.ToArray();
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                CelestialIndex = stream.ReadInt();
                moons = stream.ReadArray(s => new EveMoon.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteInt(CelestialIndex);
                stream.WriteArray(moons, m => m.Write(stream));
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public int CelestialIndex => data.CelestialIndex;

        public string Name { get; }

        public EveSolarSystem SolarSystem { get; }
        public EvePlanetMoons Moons { get; }

        public EvePlanet(Data data, EveSolarSystem solarSystem, EveData eveData)
        {
            this.data = data;

            Name = $"{solarSystem.Name} {RomanNumbers.Format(CelestialIndex)}";

            SolarSystem = solarSystem;
            Moons = new(data.Moons.Select(d => new EveMoon(d, this, eveData)));
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EveAllPlanets : IEnumerable<EvePlanet>
    {
        private readonly EvePlanet[] planets;

        private readonly Lazy<Dictionary<int, EvePlanet>> byId;
        private readonly Lazy<Dictionary<string, EvePlanet>> byName;

        public int Count => planets.Length;

        public EvePlanet this[int id] => byId.Value[id];
        public EvePlanet this[string name] => byName.Value[name];

        public EveAllPlanets(IEnumerable<EvePlanet> planets)
        {
            this.planets = planets.ToArray();

            byId = new(() => this.planets.ToDictionary(x => x.Id), true);
            byName = new(() => this.planets.ToDictionary(x => x.Name), true);
        }

        public IEnumerator<EvePlanet> GetEnumerator()
            => planets.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => planets.GetEnumerator();
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EveSystemPlanets : IEnumerable<EvePlanet>
    {
        private readonly EvePlanet[] planets;

        private readonly Lazy<Dictionary<int, EvePlanet>> byId;
        private readonly Lazy<Dictionary<int, EvePlanet>> byIndex;
        private readonly Lazy<Dictionary<string, EvePlanet>> byName;

        public int Count => planets.Length;

        public EvePlanet this[int idOrIndex] => GetPlanet(idOrIndex);
        public EvePlanet this[string name] => byName.Value[name];

        public EveSystemPlanets(IEnumerable<EvePlanet> planets)
        {
            this.planets = planets.ToArray();

            byId = new(() => this.planets.ToDictionary(x => x.Id), true);
            byIndex = new(() => this.planets.ToDictionary(x => x.CelestialIndex), true);
            byName = new(() => this.planets.ToDictionary(x => x.Name), true);
        }

        private EvePlanet GetPlanet(int idOrIndex)
        {
            if (byIndex.Value.TryGetValue(idOrIndex, out EvePlanet? planet))
            {
                return planet;
            }

            return byId.Value[idOrIndex];
        }

        public IEnumerator<EvePlanet> GetEnumerator()
            => planets.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => planets.GetEnumerator();
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
        private readonly EveStargate[] stargates;
        private readonly Lazy<Dictionary<int, EveStargate>> byId;

        public int Count => stargates.Length;

        public EveStargate this[int id] => byId.Value[id];

        public EveStargates(IEnumerable<EveStargate> stargates)
        {
            this.stargates = stargates.ToArray();

            byId = new(() => this.stargates.ToDictionary(x => x.Id), true); ;
        }

        public IEnumerator<EveStargate> GetEnumerator()
            => stargates.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => stargates.GetEnumerator();
    }

    [DebuggerDisplay("{Name}")]
    public class EveSolarSystem
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; }
            public double Security { get; }

            private readonly EvePlanet.Data[] planets;
            public IReadOnlyList<EvePlanet.Data> Planets => planets;

            private readonly EveStargate.Data[] stargates;
            public IReadOnlyList<EveStargate.Data> Stargates => stargates;

            public Data(int id, string name, double security, IEnumerable<EvePlanet.Data> planets, IEnumerable<EveStargate.Data> stargates)
            {
                Id = id;
                Name = name;
                Security = security;
                this.planets = planets.ToArray();
                this.stargates = stargates.ToArray();
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Name = stream.ReadString();
                Security = stream.ReadDouble();
                planets = stream.ReadArray(s => new EvePlanet.Data(s));
                stargates = stream.ReadArray(s => new EveStargate.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteString(Name);
                stream.WriteDouble(Security);
                stream.WriteArray(planets, p => p.Write(stream));
                stream.WriteArray(stargates, sg => sg.Write(stream));
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public string Name => data.Name;

        public EveConstellation Constellation { get; }

        public EveSystemPlanets Planets { get; }
        public EveStargates Stargates { get; }

        public EveSolarSystem(Data data, EveConstellation constellation, EveData eveData)
        {
            this.data = data;

            Constellation = constellation;

            Planets = new(data.Planets.Select(d => new EvePlanet(d, this, eveData)));
            Stargates = new(data.Stargates.Select(d => new EveStargate(d)));
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EveSolarSystems : IEnumerable<EveSolarSystem>
    {
        private readonly EveSolarSystem[] solarSystems;

        private readonly Lazy<Dictionary<int, EveSolarSystem>> byId;
        private readonly Lazy<Dictionary<string, EveSolarSystem>> byName;

        public int Count => solarSystems.Length;

        public EveSolarSystem this[int id] => byId.Value[id];
        public EveSolarSystem this[string name] => byName.Value[name];

        public EveSolarSystems(IEnumerable<EveSolarSystem> solarSystems)
        {
            this.solarSystems = solarSystems.ToArray();

            byId = new(() => this.solarSystems.ToDictionary(x => x.Id), true);
            byName = new(() => this.solarSystems.ToDictionary(x => x.Name), true);
        }

        public IEnumerator<EveSolarSystem> GetEnumerator()
            => solarSystems.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => solarSystems.GetEnumerator();
    }

    [DebuggerDisplay("{Name}")]
    public class EveConstellation
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; }

            private EveSolarSystem.Data[] solarSystems;
            public IReadOnlyList<EveSolarSystem.Data> SolarSystems => solarSystems;

            public Data(int id, string name, IEnumerable<EveSolarSystem.Data> solarSystems)
            {
                Id = id;
                Name = name;
                this.solarSystems = solarSystems.ToArray();
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Name = stream.ReadString();
                solarSystems = stream.ReadArray(s => new EveSolarSystem.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteString(Name);
                stream.WriteArray(solarSystems, s => s.Write(stream));
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public string Name => data.Name;

        public EveRegion Region { get; }
        public EveSolarSystems SolarSystems { get; }

        public EveConstellation(Data data, EveRegion region, EveData eveData)
        {
            this.data = data;

            Region = region;
            SolarSystems = new(data.SolarSystems.Select(d => new EveSolarSystem(d, this, eveData)));
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EveConstellations : IEnumerable<EveConstellation>
    {
        private readonly EveConstellation[] constellations;
        private readonly Lazy<Dictionary<int, EveConstellation>> byId;
        private readonly Lazy<Dictionary<string, EveConstellation>> byName;

        public int Count => constellations.Length;

        public EveConstellation this[int id] => byId.Value[id];
        public EveConstellation this[string name] => byName.Value[name];

        public EveConstellations(IEnumerable<EveConstellation> constellations)
        {
            this.constellations = constellations.ToArray();

            byId = new(() => this.constellations.ToDictionary(x => x.Id), true);
            byName = new(() => this.constellations.ToDictionary(x => x.Name), true);
        }

        public IEnumerator<EveConstellation> GetEnumerator()
            => constellations.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => constellations.GetEnumerator();
    }

    [DebuggerDisplay("{Name}")]
    public class EveRegion
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; }

            private readonly EveConstellation.Data[] constellations;
            public IReadOnlyList<EveConstellation.Data> Constellations => constellations;

            public Data(int id, string name, IEnumerable<EveConstellation.Data> constellations)
            {
                Id = id;
                Name = name;
                this.constellations = constellations.ToArray();
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Name = stream.ReadString();
                constellations = stream.ReadArray(s => new EveConstellation.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteString(Name);
                stream.WriteArray(constellations, c => c.Write(stream));
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public string Name => data.Name;

        public EveConstellations Constellations { get; }

        public EveRegion(Data data, EveData eveData)
        {
            this.data = data;

            Constellations = new(data.Constellations.Select(d => new EveConstellation(d, this, eveData)));
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EveRegions : IEnumerable<EveRegion>
    {
        private readonly EveRegion[] regions;

        private readonly Lazy<Dictionary<int, EveRegion>> byId;
        private readonly Lazy<Dictionary<string, EveRegion>> byName;

        public int Count => regions.Length;

        public EveRegion this[int id] => byId.Value[id];
        public EveRegion this[string name] => byName.Value[name];

        public EveRegions(IEnumerable<EveRegion> regions)
        {
            this.regions = regions.ToArray();

            byId = new(() => this.regions.ToDictionary(x => x.Id), true);
            byName = new(() => this.regions.ToDictionary(x => x.Name), true);
        }

        public IEnumerator<EveRegion> GetEnumerator()
            => regions.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => regions.GetEnumerator();
    }

    public class EveUniverse
    {
        public class Data
        {
            private readonly EveRegion.Data[] regions;
            public IReadOnlyList<EveRegion.Data> Regions => regions;

            public Data(IEnumerable<EveRegion.Data> regions)
            {
                this.regions = regions.ToArray();
            }

            public Data(Stream stream)
            {
                regions = stream.ReadArray(s => new EveRegion.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteArray(regions, r => r.Write(stream));
            }
        }

        private readonly Data data;

        public EveRegions Regions { get; }
        public EveConstellations Constellations { get; }
        public EveSolarSystems SolarSystems { get; }
        public EveAllPlanets Planets { get; }
        public EveAllMoons Moons { get; }
        public EveMoonStations NpcStations { get; }
        public EveStargates Stargates { get; }

        public EveUniverse(Data data, EveData eveData)
        {
            this.data = data;

            Regions = new(data.Regions.AsParallel().Select(d => new EveRegion(d, eveData)));
            Constellations = new(Regions.SelectMany(r => r.Constellations));
            SolarSystems = new(Constellations.SelectMany(c => c.SolarSystems));
            Planets = new(SolarSystems.SelectMany(s => s.Planets));
            Moons = new(Planets.SelectMany(p => p.Moons));
            NpcStations = new(Moons.SelectMany(m => m.Stations));
            Stargates = new(SolarSystems.SelectMany(s => s.Stargates));
        }

        public EveUniverse(Stream stream, EveData eveData)
            : this(new Data(stream), eveData) { }

        public void Write(Stream stream)
        {
            data.Write(stream);
        }
    }
}
