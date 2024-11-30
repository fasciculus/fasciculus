using System.IO;

namespace Fasciculus.Eve.Models
{
    public class EveSolarSystem
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; }
            public double Security { get; }

            public Data(int id, string name, double security)
            {
                Id = id;
                Name = name;
                Security = security;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Name = stream.ReadString();
                Security = stream.ReadDouble();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteString(Name);
                stream.WriteDouble(Security);
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

        public EveRegion(Stream stream)
            : this(new Data(stream)) { }

        public void Write(Stream stream)
        {
            data.Write(stream);
        }
    }
}
