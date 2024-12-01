using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
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

    public class EveStargates : IEnumerable<EveStargate>
    {
        private readonly Dictionary<int, EveStargate> byId;

        public int Count => byId.Count;

        public EveStargate this[int id] => byId[id];

        public EveStargates(IEnumerable<EveStargate> stargates)
        {
            byId = stargates.ToDictionary(r => r.Id, r => r);
        }

        public IEnumerator<EveStargate> GetEnumerator()
            => byId.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => byId.Values.GetEnumerator();
    }

    public class EveSolarSystem
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; }
            public double Security { get; }

            public EveStargate.Data[] Stargates { get; }

            public Data(int id, string name, double security, EveStargate.Data[] stargates)
            {
                Id = id;
                Name = name;
                Security = security;
                Stargates = stargates;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Name = stream.ReadString();
                Security = stream.ReadDouble();
                Stargates = stream.ReadArray(s => new EveStargate.Data(s));
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteString(Name);
                stream.WriteDouble(Security);
                stream.WriteArray(Stargates, sg => sg.Write(stream));
            }
        }

        private readonly Data data;

        public int Id => data.Id;
        public string Name => data.Name;

        public EveStargates Stargates { get; }

        public EveSolarSystem(Data data)
        {
            this.data = data;

            Stargates = new(data.Stargates.Select(d => new EveStargate(d)));
        }
    }

    public class EveSolarSystems : IEnumerable<EveSolarSystem>
    {
        private readonly Dictionary<int, EveSolarSystem> byId;
        private readonly Dictionary<string, EveSolarSystem> byName;

        public int Count => byId.Count;

        public EveSolarSystem this[int id] => byId[id];
        public EveSolarSystem this[string name] => byName[name];

        public EveSolarSystems(IEnumerable<EveSolarSystem> solarSystems)
        {
            byId = solarSystems.ToDictionary(r => r.Id, r => r);
            byName = solarSystems.ToDictionary(r => r.Name, r => r);
        }

        public IEnumerator<EveSolarSystem> GetEnumerator()
            => byId.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => byId.Values.GetEnumerator();
    }

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

    public class EveConstellations : IEnumerable<EveConstellation>
    {
        private readonly Dictionary<int, EveConstellation> byId;
        private readonly Dictionary<string, EveConstellation> byName;

        public int Count => byId.Count;

        public EveConstellation this[int id] => byId[id];
        public EveConstellation this[string name] => byName[name];

        public EveConstellations(IEnumerable<EveConstellation> constellations)
        {
            byId = constellations.ToDictionary(r => r.Id, r => r);
            byName = constellations.ToDictionary(r => r.Name, r => r);
        }

        public IEnumerator<EveConstellation> GetEnumerator()
            => byId.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => byId.Values.GetEnumerator();
    }

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

    public class EveRegions : IEnumerable<EveRegion>
    {
        private readonly Dictionary<int, EveRegion> byId;
        private readonly Dictionary<string, EveRegion> byName;

        public int Count => byId.Count;

        public EveRegion this[int id] => byId[id];
        public EveRegion this[string name] => byName[name];

        public EveRegions(IEnumerable<EveRegion> regions)
        {
            byId = regions.ToDictionary(r => r.Id, r => r);
            byName = regions.ToDictionary(r => r.Name, r => r);
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
        public EveStargates Stargates { get; }

        public EveUniverse(Data data)
        {
            this.data = data;

            Regions = new(data.Regions.Select(d => new EveRegion(d)));
            Constellations = new(Regions.SelectMany(r => r.Constellations));
            SolarSystems = new(Constellations.SelectMany(c => c.SolarSystems));
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
