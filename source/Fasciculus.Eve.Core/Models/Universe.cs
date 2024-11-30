using System.IO;

namespace Fasciculus.Eve.Models
{
    public class EveConstellation
    {
        public class Data
        {
            public int Id { get; }
            public string Name { get; }

            public Data(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Name = stream.ReadString();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteString(Name);
            }
        }

        private readonly Data data;

        public EveConstellation(Data data)
        {
            this.data = data;
        }

        public EveConstellation(Stream stream)
            : this(new Data(stream)) { }
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

        public EveRegion(Data data)
        {
            this.data = data;
        }

        public EveRegion(Stream stream)
            : this(new Data(stream)) { }
    }
}
