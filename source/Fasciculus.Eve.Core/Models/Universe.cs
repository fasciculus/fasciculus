using System.IO;

namespace Fasciculus.Eve.Models
{
    public class EveRegion
    {
        public class Data
        {
            public int Id { get; }

            public Data(int id)
            {
                Id = id;
            }

            public Data(Stream stream)
            {
                Id = stream.ReadInt();
            }

            public void Write(Stream stream)
            {
                stream.WriteInt(Id);
            }
        }

        private readonly Data data;

        public int Id => data.Id;

        public EveRegion(Data data)
        {
            this.data = data;
        }

        public EveRegion(Stream stream)
        {
            data = new Data(stream);
        }
    }
}
