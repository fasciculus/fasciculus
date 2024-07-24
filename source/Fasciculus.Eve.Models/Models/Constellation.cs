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

        public void Write(Data data)
        {
            data.WriteInt(Id);
            data.WriteInt(region);
        }
    }
}
