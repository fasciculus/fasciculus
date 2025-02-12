using Fasciculus.IO;
using Fasciculus.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class EveStation
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

            public Data(BinaryRW bin)
            {
                Id = bin.ReadInt64();
                Operation = bin.ReadInt32();
                Owner = bin.ReadInt32();
            }

            public void Write(BinaryRW bin)
            {
                bin.WriteInt64(Id);
                bin.WriteInt32(Operation);
                bin.WriteInt32(Owner);
            }
        }

        private readonly Data data;

        public long Id => data.Id;

        public string Name { get; }
        public string FullName { get; }

        public EveMoon Moon { get; }
        public EveStationOperation Operation { get; }
        public EveNpcCorporation Owner { get; }

        public double Security => Moon.Security;

        public EveStation(Data data, EveMoon moon, EveData eveData)
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
    public class EveStations : IEnumerable<EveStation>
    {
        private readonly EveStation[] stations;

        private readonly Lazy<Dictionary<long, EveStation>> byId;

        public int Count => stations.Length;

        public bool Contains(long id) => byId.Value.ContainsKey(id);
        public EveStation this[long id] => byId.Value[id];

        public EveStations(IEnumerable<EveStation> stations)
        {
            this.stations = stations.ToArray();

            byId = new(() => this.stations.ToDictionary(x => x.Id), true);
        }

        public IEnumerator<EveStation> GetEnumerator()
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

            private readonly EveStation.Data[] stations;
            public IReadOnlyList<EveStation.Data> Stations => stations;

            public Data(int id, int celestialIndex, IEnumerable<EveStation.Data> stations)
            {
                Id = id;
                CelestialIndex = celestialIndex;
                this.stations = stations.ToArray();
            }

            public Data(BinaryRW bin)
            {
                Id = bin.ReadInt32();
                CelestialIndex = bin.ReadInt32();
                stations = bin.ReadArray(() => new EveStation.Data(bin));
            }

            public void Write(BinaryRW bin)
            {
                bin.WriteInt32(Id);
                bin.WriteInt32(CelestialIndex);
                bin.WriteArray(stations, x => x.Write(bin));
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public int CelestialIndex => data.CelestialIndex;

        public string Name { get; }

        public EvePlanet Planet { get; }
        public EveStations Stations { get; }

        public double Security => Planet.Security;

        public EveMoon(Data data, EvePlanet planet, EveData eveData)
        {
            this.data = data;

            Name = $"{planet.Name} - Moon {CelestialIndex}";

            Planet = planet;
            Stations = new(data.Stations.Select(d => new EveStation(d, this, eveData)));
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

            public Data(BinaryRW bin)
            {
                Id = bin.ReadInt32();
                CelestialIndex = bin.ReadInt32();
                moons = bin.ReadArray(() => new EveMoon.Data(bin));
            }

            public void Write(BinaryRW bin)
            {
                bin.WriteInt32(Id);
                bin.WriteInt32(CelestialIndex);
                bin.WriteArray(moons, m => m.Write(bin));
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public int CelestialIndex => data.CelestialIndex;

        public string Name { get; }

        public EveSolarSystem SolarSystem { get; }
        public EvePlanetMoons Moons { get; }

        public double Security => SolarSystem.Security;

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
            public uint Id { get; }
            public uint Destination { get; }

            public Data(uint id, uint destination)
            {
                Id = id;
                Destination = destination;
            }

            public Data(BinaryRW bin)
            {
                Id = bin.ReadUInt32();
                Destination = bin.ReadUInt32();
            }

            public void Write(BinaryRW bin)
            {
                bin.WriteUInt32(Id);
                bin.WriteUInt32(Destination);
            }
        }

        private readonly Data data;

        public uint Id => data.Id;

        public EveStargate(Data data)
        {
            this.data = data;
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    public class EveStargates : IEnumerable<EveStargate>
    {
        private readonly EveStargate[] stargates;
        private readonly Lazy<Dictionary<uint, EveStargate>> byId;

        public int Count => stargates.Length;

        public EveStargate this[uint id] => byId.Value[id];

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
            public uint Id { get; }
            public string Name { get; }
            public double Security { get; }

            private readonly EvePlanet.Data[] planets;
            public IReadOnlyList<EvePlanet.Data> Planets => planets;

            private readonly EveStargate.Data[] stargates;
            public IReadOnlyList<EveStargate.Data> Stargates => stargates;

            public Data(uint id, string name, double security, IEnumerable<EvePlanet.Data> planets, IEnumerable<EveStargate.Data> stargates)
            {
                Id = id;
                Name = name;
                Security = security;
                this.planets = planets.ToArray();
                this.stargates = stargates.ToArray();
            }

            public Data(BinaryRW bin)
            {
                Id = bin.ReadUInt32();
                Name = bin.ReadString();
                Security = bin.ReadDouble();
                planets = bin.ReadArray(() => new EvePlanet.Data(bin));
                stargates = bin.ReadArray(() => new EveStargate.Data(bin));
            }

            public void Write(BinaryRW bin)
            {
                bin.WriteUInt32(Id);
                bin.WriteString(Name);
                bin.WriteDouble(Security);
                bin.WriteArray(planets, p => p.Write(bin));
                bin.WriteArray(stargates, sg => sg.Write(bin));
            }
        }

        private readonly Data data;

        public uint Id => data.Id;
        public string Name => data.Name;
        public double Security => data.Security;

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

        private readonly Lazy<Dictionary<uint, EveSolarSystem>> byId;
        private readonly Lazy<Dictionary<string, EveSolarSystem>> byName;

        public int Count => solarSystems.Length;

        public bool Contains(uint id) => byId.Value.ContainsKey(id);
        public bool Contains(string name) => byName.Value.ContainsKey(name);

        public EveSolarSystem this[uint id] => byId.Value[id];
        public EveSolarSystem this[string name] => byName.Value[name];

        public EveSolarSystems this[EveSecurity.Level level] => new(OfSecurity(level));

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

        private EveSolarSystem[] OfSecurity(EveSecurity.Level level)
        {
            EveSecurity.Filter filter = EveSecurity.Filters[level];

            return solarSystems.Where(x => filter(x.Security)).ToArray();
        }
    }

    [DebuggerDisplay("{Name}")]
    public class EveConstellation
    {
        public class Data
        {
            public uint Id { get; }
            public string Name { get; }

            private EveSolarSystem.Data[] solarSystems;
            public IReadOnlyList<EveSolarSystem.Data> SolarSystems => solarSystems;

            public Data(uint id, string name, IEnumerable<EveSolarSystem.Data> solarSystems)
            {
                Id = id;
                Name = name;
                this.solarSystems = solarSystems.ToArray();
            }

            public Data(BinaryRW bin)
            {
                Id = bin.ReadUInt32();
                Name = bin.ReadString();
                solarSystems = bin.ReadArray(() => new EveSolarSystem.Data(bin));
            }

            public void Write(BinaryRW bin)
            {
                bin.WriteUInt32(Id);
                bin.WriteString(Name);
                bin.WriteArray(solarSystems, s => s.Write(bin));
            }
        }

        private readonly Data data;

        public uint Id => data.Id;
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
        private readonly Lazy<Dictionary<uint, EveConstellation>> byId;
        private readonly Lazy<Dictionary<string, EveConstellation>> byName;

        public int Count => constellations.Length;

        public EveConstellation this[uint id] => byId.Value[id];
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
            public uint Id { get; }
            public string Name { get; }

            private readonly EveConstellation.Data[] constellations;
            public IReadOnlyList<EveConstellation.Data> Constellations => constellations;

            public Data(uint id, string name, IEnumerable<EveConstellation.Data> constellations)
            {
                Id = id;
                Name = name;
                this.constellations = constellations.ToArray();
            }

            public Data(BinaryRW bin)
            {
                Id = bin.ReadUInt32();
                Name = bin.ReadString();
                constellations = bin.ReadArray(() => new EveConstellation.Data(bin));
            }

            public void Write(BinaryRW bin)
            {
                bin.WriteUInt32(Id);
                bin.WriteString(Name);
                bin.WriteArray(constellations, c => c.Write(bin));
            }
        }

        private readonly Data data;

        public uint Id => data.Id;
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

        private readonly Lazy<Dictionary<uint, EveRegion>> byId;
        private readonly Lazy<Dictionary<string, EveRegion>> byName;

        public int Count => regions.Length;

        public EveRegion this[uint id] => byId.Value[id];
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

    public static class EveUniverseExtensions
    {
        public static EveRegion GetRegion(this EveStation station)
            => station.Moon.Planet.SolarSystem.Constellation.Region;
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

            public Data(BinaryRW bin)
            {
                regions = bin.ReadArray(() => new EveRegion.Data(bin));
            }

            public void Write(BinaryRW bin)
            {
                bin.WriteArray(regions, r => r.Write(bin));
            }
        }

        private readonly Data data;

        public EveRegions Regions { get; }
        public EveConstellations Constellations { get; }
        public EveSolarSystems SolarSystems { get; }
        public EveAllPlanets Planets { get; }
        public EveAllMoons Moons { get; }
        public EveStations Stations { get; }
        public EveStargates Stargates { get; }

        public EveUniverse(Data data, EveData eveData)
        {
            this.data = data;

            Regions = new(data.Regions.AsParallel().Select(d => new EveRegion(d, eveData)));
            Constellations = new(Regions.SelectMany(r => r.Constellations));
            SolarSystems = new(Constellations.SelectMany(c => c.SolarSystems));
            Planets = new(SolarSystems.SelectMany(s => s.Planets));
            Moons = new(Planets.SelectMany(p => p.Moons));
            Stations = new(Moons.SelectMany(m => m.Stations));
            Stargates = new(SolarSystems.SelectMany(s => s.Stargates));
        }

        public EveUniverse(BinaryRW bin, EveData eveData)
            : this(new Data(bin), eveData) { }

        public void Write(BinaryRW bin)
        {
            data.Write(bin);
        }
    }
}
