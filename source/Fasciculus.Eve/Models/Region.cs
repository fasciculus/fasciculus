using Fasciculus.Eve.Models.Sde;

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
    }
}
