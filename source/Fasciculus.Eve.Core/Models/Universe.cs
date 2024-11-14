using Fasciculus.Algorithms;
using Fasciculus.Models;
using Fasciculus.Validating;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveCelestialIndex : Id<int>
    {
        public EveCelestialIndex(int value)
            : base(value) { }

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
    }

    public class EveNpcStation : EveObject
    {
        private readonly EveId operationId;
        private readonly EveId ownerId;
        private readonly EveId typeId;

        private EveNpcCorporation? owner;

        public EveNpcCorporation Owner
            => Cond.NotNull(owner);

        public EveNpcStation(EveId id, EveId operationId, EveId ownerId, EveId typeId)
            : base(id)
        {
            this.operationId = operationId;
            this.ownerId = ownerId;
            this.typeId = typeId;
        }

        internal void Link(EveData data)
        {
            owner = data.NpcCorporations[ownerId];
        }

        public void Write(Stream stream)
        {
            Id.Write(stream);
            operationId.Write(stream);
            ownerId.Write(stream);
            typeId.Write(stream);
        }

        public static EveNpcStation Read(Stream stream)
        {
            EveId id = EveId.Read(stream);
            EveId operationId = EveId.Read(stream);
            EveId ownerId = EveId.Read(stream);
            EveId typeId = EveId.Read(stream);

            return new(id, operationId, ownerId, typeId);
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

        public void Link(EvePlanet planet, EveData data)
        {
            this.planet = planet;
            npcStations.Apply(s => s.Link(data));
        }

        public void Write(Stream stream)
        {
            Id.Write(stream);
            CelestialIndex.Write(stream);
            stream.WriteArray(npcStations, npcStation => npcStation.Write(stream));
        }

        public static EveMoon Read(Stream stream)
        {
            EveId id = EveId.Read(stream);
            EveCelestialIndex celestialIndex = EveCelestialIndex.Read(stream);
            EveNpcStation[] npcStations = stream.ReadArray(EveNpcStation.Read);

            return new(id, celestialIndex, npcStations);
        }
    }

    public class EveMoons : EveObjects<EveMoon>
    {
        private readonly Dictionary<EveCelestialIndex, EveMoon> moons;

        public EveMoons(EveMoon[] moons)
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

        internal void Link(EveSolarSystem solarSystem, EveData data)
        {
            this.solarSystem = solarSystem;
            Moons.Apply(moon => moon.Link(this, data));
        }

        public void Write(Stream stream)
        {
            Id.Write(stream);
            CelestialIndex.Write(stream);
            Moons.Write(stream);
        }

        public static EvePlanet Read(Stream stream)
        {
            EveId id = EveId.Read(stream);
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

        public EvePlanets(EvePlanet[] planets)
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

        public void Write(Stream stream)
        {
            Id.Write(stream);
            destinationId.Write(stream);
        }

        public static EveStargate Read(Stream stream)
        {
            EveId id = EveId.Read(stream);
            EveId destinationId = EveId.Read(stream);

            return new EveStargate(id, destinationId);
        }
    }

    public class EveStargates : EveObjects<EveStargate>
    {
        public EveStargates(EveStargate[] stargates)
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

        public EveSolarSystem(EveId id, EveNames names, double security, EveStargate[] stargates, EvePlanets planets, bool hasIce)
            : base(id, names)
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

        public void Link(EveConstellation constellation, EveData data, EveUniverse universe)
        {
            this.constellation = constellation;
            stargates.Apply(stargate => stargate.Link(this, universe));
            Planets.Apply(planet => planet.Link(this, data));
        }

        public void Write(Stream stream)
        {
            Id.Write(stream);
            stream.WriteDouble(Security);
            stream.WriteArray(stargates, stargate => stargate.Write(stream));
            Planets.Write(stream);
            stream.WriteBool(HasIce);
        }

        public static EveSolarSystem Read(Stream stream, EveNames names)
        {
            EveId id = EveId.Read(stream);
            double security = stream.ReadDouble();
            EveStargate[] stargates = stream.ReadArray(EveStargate.Read);
            EvePlanets planets = EvePlanets.Read(stream);
            bool hasIce = stream.ReadBool();

            return new(id, names, security, stargates, planets, hasIce);
        }

        public override string ToString()
        {
            return $"{Name} ({EveSecurity.Format(Security)}) {Constellation.Region.Name}";
        }
    }

    public class EveSolarSystems : EveNamedObjects<EveSolarSystem>
    {
        public EveSolarSystems(EveSolarSystem[] solarSystems)
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

        public EveConstellation(EveId id, EveNames names, EveSolarSystem[] solarSystems)
            : base(id, names)
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

        internal void Link(EveRegion region, EveData data, EveUniverse universe)
        {
            this.region = region;
            solarSystems.Apply(solarSystem => solarSystem.Link(this, data, universe));
        }

        public void Write(Stream stream)
        {
            Id.Write(stream);
            stream.WriteArray(solarSystems, solarSystem => solarSystem.Write(stream));
        }

        public static EveConstellation Read(Stream stream, EveNames names)
        {
            EveId id = EveId.Read(stream);
            EveSolarSystem[] solarSystems = stream.ReadArray(_ => EveSolarSystem.Read(stream, names));

            return new(id, names, solarSystems);
        }
    }

    public class EveConstellations : EveNamedObjects<EveConstellation>
    {
        public EveConstellations(EveConstellation[] constellations)
            : base(constellations) { }
    }

    public class EveRegion : EveNamedObject
    {
        private readonly EveConstellation[] constellations;

        public IEnumerable<EveConstellation> Constellations
            => constellations;

        public EveRegion(EveId id, EveNames names, EveConstellation[] constellations)
            : base(id, names)
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

        internal void Link(EveData data, EveUniverse universe)
        {
            constellations.Apply(constellation => constellation.Link(this, data, universe));
        }

        public void Write(Stream stream)
        {
            Id.Write(stream);
            stream.WriteArray(constellations, constellation => constellation.Write(stream));
        }

        public static EveRegion Read(Stream stream, EveNames names)
        {
            EveId id = EveId.Read(stream);
            EveConstellation[] constellations = stream.ReadArray(_ => EveConstellation.Read(stream, names));

            return new EveRegion(id, names, constellations);
        }
    }

    public class EveRegions : EveNamedObjects<EveRegion>
    {
        public EveRegions(EveRegion[] regions)
            : base(regions) { }

        internal void Link(EveData data, EveUniverse universe)
        {
            this.Apply(region => region.Link(data, universe));
        }

        public void Write(Stream stream)
        {
            stream.WriteArray(objectsByIndex, o => o.Write(stream));
        }

        public static EveRegions Read(Stream stream, EveNames names)
        {
            EveRegion[] regions = stream.ReadArray(_ => EveRegion.Read(stream, names));

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

        public EveUniverse(EveData data, EveRegions regions)
        {
            Regions = regions;
            Constellations = new(Regions.SelectMany(r => r.Constellations).ToArray());
            SolarSystems = new(Constellations.SelectMany(c => c.SolarSystems).ToArray());
            Stargates = new(SolarSystems.SelectMany(s => s.Stargates).ToArray());

            Regions.Link(data, this);
        }

        public void Write(Stream stream)
        {
            Regions.Write(stream);
        }

        public static EveUniverse Read(Stream stream, EveData data)
        {
            EveRegions regions = EveRegions.Read(stream, data.Names);

            return new(data, regions);
        }
    }
}
