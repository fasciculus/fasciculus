using Fasciculus.Validating;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
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
        private readonly EveNpcStation[] npcStations;

        public EveMoon(EveId id, EveNpcStation[] npcStations)
            : base(id)
        {
            this.npcStations = npcStations;
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);

            stream.WriteArray(npcStations, npcStation => npcStation.Write(stream));
        }

        public static EveMoon Read(Stream stream)
        {
            EveId id = BaseRead(stream);
            EveNpcStation[] npcStations = stream.ReadArray(EveNpcStation.Read);

            return new(id, npcStations);
        }
    }

    public class EvePlanet : EveObject
    {
        private readonly EveMoon[] moons;

        public int CelestialIndex { get; }

        public IEnumerable<EveMoon> Moons
            => moons;

        public EvePlanet(EveId id, int celestialIndex, EveMoon[] moons)
            : base(id)
        {
            CelestialIndex = celestialIndex;
            this.moons = moons;
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);

            stream.WriteInt(CelestialIndex);
            stream.WriteArray(moons, moon => moon.Write(stream));
        }

        public static EvePlanet Read(Stream stream)
        {
            EveId id = BaseRead(stream);
            int celestialIndex = stream.ReadInt();
            EveMoon[] moons = stream.ReadArray(EveMoon.Read);

            return new(id, celestialIndex, moons);
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
        private readonly EvePlanet[] planets;

        public EveConstellation Constellation
            => Cond.NotNull(constellation);

        public IEnumerable<EveStargate> Stargates
            => stargates;

        public IEnumerable<EvePlanet> Planets
            => planets;

        public bool HasIce { get; private set; }

        public EveSolarSystem(EveId id, string name, double security, EveStargate[] stargates, EvePlanet[] planets, bool hasIce)
            : base(id, name)
        {
            Security = security;
            this.stargates = stargates;
            this.planets = planets;
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
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);

            stream.WriteDouble(Security);
            stream.WriteArray(stargates, stargate => stargate.Write(stream));
            stream.WriteArray(planets, planet => planet.Write(stream));
            stream.WriteBool(HasIce);
        }

        public static EveSolarSystem Read(Stream stream)
        {
            (EveId id, string name) = BaseRead(stream);
            double security = stream.ReadDouble();
            EveStargate[] stargates = stream.ReadArray(EveStargate.Read);
            EvePlanet[] planets = stream.ReadArray(EvePlanet.Read);
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
