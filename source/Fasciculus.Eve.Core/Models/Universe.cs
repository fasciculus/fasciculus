using Fasciculus.Algorithms;
using Fasciculus.Validating;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveCelestialIndex : IEquatable<EveCelestialIndex>, IComparable<EveCelestialIndex>
    {
        public readonly int Value;

        public EveCelestialIndex(int value)
        {
            Value = value;
        }

        public static EveCelestialIndex Create(int id)
            => new(id);

        public void Write(Stream stream)
        {
            stream.WriteInt(Value);
        }

        public static EveCelestialIndex Read(Stream stream)
        {
            return new(stream.ReadInt());
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
            => obj is EveCelestialIndex id && Value == id.Value;

        public bool Equals(EveCelestialIndex? other)
            => other is not null && Value == other.Value;

        public int CompareTo(EveCelestialIndex? other)
            => other is not null ? Value.CompareTo(other.Value) : -1;

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string? ToString()
        {
            return Value.ToString();
        }

        public static bool operator ==(EveCelestialIndex a, EveCelestialIndex b)
            => a.Value == b.Value;

        public static bool operator !=(EveCelestialIndex a, EveCelestialIndex b)
            => a.Value == b.Value;

        public static bool operator <(EveCelestialIndex a, EveCelestialIndex b)
            => a.Value < b.Value;

        public static bool operator >(EveCelestialIndex a, EveCelestialIndex b)
            => a.Value > b.Value;

        public static bool operator <=(EveCelestialIndex a, EveCelestialIndex b)
            => a.Value <= b.Value;

        public static bool operator >=(EveCelestialIndex a, EveCelestialIndex b)
            => a.Value >= b.Value;
    }

    public class EveNpcStation : EveObject
    {
        private readonly EveId operationId;
        private readonly EveId typeId;

        public EveNpcStation(EveId id, EveId operationId, EveId typeId)
            : base(id)
        {
            this.operationId = operationId;
            this.typeId = typeId;
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);

            operationId.Write(stream);
            typeId.Write(stream);
        }

        public static EveNpcStation Read(Stream stream)
        {
            EveId id = BaseRead(stream);
            EveId operationId = EveId.Read(stream);
            EveId typeId = EveId.Read(stream);

            return new(id, operationId, typeId);
        }
    }

    public class EveMoon : EveObject
    {
        private EvePlanet? planet;

        public EvePlanet Planet
            => Cond.NotNull(planet);

        public EveCelestialIndex CelestialIndex { get; }

        private readonly EveNpcStation[] npcStations;

        public string Name
            => $"{Planet.Name} Moon {CelestialIndex.Value}";

        public EveMoon(EveId id, EveCelestialIndex celestialIndex, EveNpcStation[] npcStations)
            : base(id)
        {
            CelestialIndex = celestialIndex;
            this.npcStations = npcStations;
        }

        public void Link(EvePlanet planet)
        {
            this.planet = planet;
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);

            CelestialIndex.Write(stream);
            stream.WriteArray(npcStations, npcStation => npcStation.Write(stream));
        }

        public static EveMoon Read(Stream stream)
        {
            EveId id = BaseRead(stream);
            EveCelestialIndex celestialIndex = EveCelestialIndex.Read(stream);
            EveNpcStation[] npcStations = stream.ReadArray(EveNpcStation.Read);

            return new(id, celestialIndex, npcStations);
        }
    }

    public class EveMoons : EveObjects<EveMoon>
    {
        private readonly Dictionary<EveCelestialIndex, EveMoon> moons;

        public EveMoons(IEnumerable<EveMoon> moons)
            : base(moons)
        {
            this.moons = moons.ToDictionary(moon => moon.CelestialIndex, moon => moon);
        }

        public EveMoon this[EveCelestialIndex index] => moons[index];

        public void Write(Stream stream)
        {
            stream.WriteArray(objectsByIndex, moon => moon.Write(stream));
        }

        public static EveMoons Read(Stream stream)
        {
            return new(stream.ReadArray(EveMoon.Read));
        }
    }

    public class EvePlanet : EveObject
    {
        private EveSolarSystem? solarSystem;

        public EveCelestialIndex CelestialIndex { get; }

        public EveMoons Moons { get; }

        public EveSolarSystem SolarSystem
            => Cond.NotNull(solarSystem);

        public string Name
            => $"{SolarSystem.Name} {RomanNumbers.Format(CelestialIndex.Value)}";

        public EvePlanet(EveId id, EveCelestialIndex celestialIndex, EveMoons moons)
            : base(id)
        {
            CelestialIndex = celestialIndex;
            Moons = moons;
        }

        internal void Link(EveSolarSystem solarSystem)
        {
            this.solarSystem = solarSystem;
            Moons.Apply(moon => moon.Link(this));
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);

            CelestialIndex.Write(stream);
            Moons.Write(stream);
        }

        public static EvePlanet Read(Stream stream)
        {
            EveId id = BaseRead(stream);
            EveCelestialIndex celestialIndex = EveCelestialIndex.Read(stream);
            EveMoons moons = EveMoons.Read(stream);

            return new(id, celestialIndex, moons);
        }

        public override string? ToString()
        {
            return Name;
        }
    }

    public class EvePlanets : EveObjects<EvePlanet>
    {
        private readonly Dictionary<EveCelestialIndex, EvePlanet> planets;

        public EvePlanets(IEnumerable<EvePlanet> planets)
            : base(planets)
        {
            this.planets = planets.ToDictionary(planet => planet.CelestialIndex, planet => planet);
        }

        public EvePlanet this[EveCelestialIndex index] => planets[index];

        public void Write(Stream stream)
        {
            stream.WriteArray(objectsByIndex, planet => planet.Write(stream));
        }

        public static EvePlanets Read(Stream stream)
        {
            return new(stream.ReadArray(EvePlanet.Read));
        }
    }

    public class EveStargate : EveObject
    {
        private readonly EveId destinationId;

        private EveSolarSystem? solarSystem;
        private EveStargate? destination;

        public EveSolarSystem SolarSystem => Cond.NotNull(solarSystem);
        public EveStargate Destination => Cond.NotNull(destination);

        public EveStargate(EveId id, EveId destinationId)
            : base(id)
        {
            this.destinationId = destinationId;
        }

        public void Link(EveSolarSystem solarSystem, IEveUniverse universe)
        {
            this.solarSystem = solarSystem;
            destination = universe.Stargates[destinationId];
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);

            destinationId.Write(stream);
        }

        public static EveStargate Read(Stream stream)
        {
            EveId id = BaseRead(stream);
            EveId destinationId = EveId.Read(stream);

            return new EveStargate(id, destinationId);
        }
    }

    public class EveStargates : EveObjects<EveStargate>
    {
        public EveStargates(IEnumerable<EveStargate> stargates)
            : base(stargates) { }
    }

    public class EveSolarSystem : EveNamedObject
    {
        public double Security { get; }

        private EveConstellation? constellation;
        private readonly EveStargate[] stargates;

        public EveConstellation Constellation
            => Cond.NotNull(constellation);

        public IEnumerable<EveStargate> Stargates
            => stargates;

        public EvePlanets Planets { get; }

        public bool HasIce { get; private set; }

        public EveSolarSystem(EveId id, string name, double security, EveStargate[] stargates, EvePlanets planets, bool hasIce)
            : base(id, name)
        {
            Security = security;
            this.stargates = stargates;
            Planets = planets;
            HasIce = hasIce;
        }

        public IEnumerable<EveSolarSystem> GetNeighbours(EveSecurity security)
        {
            return stargates.Select(sg => sg.Destination.SolarSystem).Where(security.Filter).OrderBy(ss => ss.Index);
        }

        public void Link(EveConstellation constellation, IEveUniverse universe)
        {
            this.constellation = constellation;
            stargates.Apply(stargate => stargate.Link(this, universe));
            Planets.Apply(planet => planet.Link(this));
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);

            stream.WriteDouble(Security);
            stream.WriteArray(stargates, stargate => stargate.Write(stream));
            Planets.Write(stream);
            stream.WriteBool(HasIce);
        }

        public static EveSolarSystem Read(Stream stream)
        {
            (EveId id, string name) = BaseRead(stream);
            double security = stream.ReadDouble();
            EveStargate[] stargates = stream.ReadArray(EveStargate.Read);
            EvePlanets planets = EvePlanets.Read(stream);
            bool hasIce = stream.ReadBool();

            return new(id, name, security, stargates, planets, hasIce);
        }

        public override string ToString()
        {
            return $"{Name} ({EveSecurity.Format(Security)}) {Constellation.Region.Name}";
        }
    }

    public class EveSolarSystems : EveNamedObjects<EveSolarSystem>
    {
        public EveSolarSystems(IEnumerable<EveSolarSystem> solarSystems)
            : base(solarSystems)
        {
        }
    }

    public class EveConstellation : EveNamedObject
    {
        private EveRegion? region;
        private readonly EveSolarSystem[] solarSystems;

        public EveRegion Region
            => Cond.NotNull(region);

        public IEnumerable<EveSolarSystem> SolarSystems
            => solarSystems;

        public EveConstellation(EveId id, string name, EveSolarSystem[] solarSystems)
            : base(id, name)
        {
            this.solarSystems = solarSystems;
        }

        public IEnumerable<EveConstellation> GetNeighbours(EveSecurity security)
        {
            return solarSystems
                .SelectMany(ss => ss.GetNeighbours(security))
                .Select(ss => ss.Constellation)
                .DistinctBy(c => c.Index)
                .Where(c => c.Index != Index)
                .OrderBy(c => c.Index);
        }

        internal void Link(EveRegion region, IEveUniverse universe)
        {
            this.region = region;
            solarSystems.Apply(solarSystem => solarSystem.Link(this, universe));
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);

            stream.WriteArray(solarSystems, solarSystem => solarSystem.Write(stream));
        }

        public static EveConstellation Read(Stream stream)
        {
            (EveId id, string name) = BaseRead(stream);
            EveSolarSystem[] solarSystems = stream.ReadArray(EveSolarSystem.Read);

            return new(id, name, solarSystems);
        }
    }

    public class EveConstellations : EveNamedObjects<EveConstellation>
    {
        public EveConstellations(IEnumerable<EveConstellation> constellations)
            : base(constellations) { }
    }

    public class EveRegion : EveNamedObject
    {
        private readonly EveConstellation[] constellations;

        public IEnumerable<EveConstellation> Constellations
            => constellations;

        public EveRegion(EveId id, string name, EveConstellation[] constellations)
            : base(id, name)
        {
            this.constellations = constellations;
        }

        public IEnumerable<EveRegion> GetNeighbours(EveSecurity security)
        {
            return constellations
                .SelectMany(c => c.GetNeighbours(security))
                .Select(c => c.Region)
                .DistinctBy(r => r.Index)
                .Where(r => r.Index != Index)
                .OrderBy(r => r.Index);
        }

        internal void Link(IEveUniverse universe)
        {
            constellations.Apply(constellation => constellation.Link(this, universe));
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);

            stream.WriteArray(constellations, constellation => constellation.Write(stream));
        }

        public static EveRegion Read(Stream stream)
        {
            (EveId id, string name) = BaseRead(stream);
            EveConstellation[] constellations = stream.ReadArray(EveConstellation.Read);

            return new EveRegion(id, name, constellations);
        }
    }

    public class EveRegions : EveNamedObjects<EveRegion>
    {
        public EveRegions(IEnumerable<EveRegion> regions)
            : base(regions) { }

        internal void Link(IEveUniverse universe)
        {
            this.Apply(region => region.Link(universe));
        }

        public void Write(Stream stream)
        {
            stream.WriteArray(objectsByIndex, o => o.Write(stream));
        }

        public static EveRegions Read(Stream stream)
        {
            EveRegion[] regions = stream.ReadArray(EveRegion.Read);

            return new(regions);
        }
    }

    public interface IEveUniverse
    {
        public EveRegions Regions { get; }
        public EveSolarSystems SolarSystems { get; }
        public EveStargates Stargates { get; }
    }

    public class EveUniverse : IEveUniverse
    {
        public EveRegions Regions { get; }
        public EveConstellations Constellations { get; }
        public EveSolarSystems SolarSystems { get; }
        public EveStargates Stargates { get; }

        public EveUniverse(EveRegions regions)
        {
            Regions = regions;
            Constellations = new(Regions.SelectMany(r => r.Constellations));
            SolarSystems = new(Constellations.SelectMany(c => c.SolarSystems));
            Stargates = new(SolarSystems.SelectMany(s => s.Stargates));

            Regions.Link(this);
        }

        public void Write(Stream stream)
        {
            Regions.Write(stream);
        }

        public static EveUniverse Read(Stream stream)
        {
            EveRegions regions = EveRegions.Read(stream);

            return new(regions);
        }
    }
}
