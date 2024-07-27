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

            Regions.Add(this);
        }

        public Region(Data data)
        {
            Id = data.ReadInt();

            Regions.Add(this);
        }

        public static void Read(Data data)
            => new Region(data);

        public void Write(Data data)
        {
            data.WriteInt(Id);
        }
    }
}
