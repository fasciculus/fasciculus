using Fasciculus.Eve.Models.Sde;
using Fasciculus.IO;

namespace Fasciculus.Eve.Models
{
    public class Constellation
    {
        public readonly int Id;

        private readonly int region;
        public Region Region => Regions.Get(region);

        public Constellation(SdeConstellation sde, int regionId)
        {
            Id = sde.constellationID;
            region = regionId;

            Constellations.Add(this);
        }

        public Constellation(Data data)
        {
            Id = data.ReadInt();
            region = data.ReadInt();

            Constellations.Add(this);
        }

        public static void Read(Data data)
            => new Constellation(data);

        public void Write(Data data)
        {
            data.WriteInt(Id);
            data.WriteInt(region);
        }
    }
}
