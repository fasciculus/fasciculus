using Fasciculus.Eve.Models.Sde;
using Fasciculus.IO;

namespace Fasciculus.Eve.Models
{
    public class Region
    {
        public readonly int Id;
        public string Name => Names.Get(Id);

        public Region(SdeRegion region)
        {
            Id = region.regionID;
        }

        public void Write(Data data)
        {
            data.WriteInt(Id);
        }
    }
}
