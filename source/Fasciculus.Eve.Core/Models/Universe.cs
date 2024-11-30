using System.IO;

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

        public EveSolarSystem(Data data)
        {
            this.data = data;
        }
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

        public EveConstellation(Data data)
        {
            this.data = data;
        }
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

        public EveRegion(Data data)
        {
            this.data = data;
        }
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

        public EveUniverse(Data data)
        {
            this.data = data;
        }

        public EveUniverse(Stream stream)
            : this(new Data(stream)) { }

        public void Write(Stream stream)
        {
            data.Write(stream);
        }
    }
}
