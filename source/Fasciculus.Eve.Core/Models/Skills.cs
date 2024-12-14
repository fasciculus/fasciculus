using System.IO;

namespace Fasciculus.Eve.Models
{
    public class EveSkill
    {
        public class Data
        {
            public int Id { get; }
            public int Level { get; }

            public Data(int id, int level)
            {
                Id = id;
                Level = level;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
                Level = stream.ReadInt();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
                stream.WriteInt(Level);
            }
        }

        private readonly Data data;

        public EveType Type { get; }
        public int Level => data.Level;

        public EveSkill(Data data, EveTypes types)
        {
            this.data = data;

            Type = types[data.Id];
        }
    }
}
