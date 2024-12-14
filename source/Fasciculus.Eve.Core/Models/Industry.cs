using System.IO;

namespace Fasciculus.Eve.Models
{
    public class EveManufacturing
    {
        public class Data
        {
            public int Time { get; }

            public Data(int time)
            {
                Time = time;
            }

            public Data(Stream stream)
            {
                Time = stream.ReadInt();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Time);
            }
        }
    }

    public class EveBlueprint
    {
        public class Data
        {
            public int Id { get; }
            public EveManufacturing.Data Manufacturing { get; }

            public Data(int id, EveManufacturing.Data manufacturing)
            {
                Id = id;
                Manufacturing = manufacturing;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Manufacturing = new EveManufacturing.Data(stream);
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                Manufacturing.Write(stream);
            }
        }
    }

    public class EveBlueprints
    {
    }
}
