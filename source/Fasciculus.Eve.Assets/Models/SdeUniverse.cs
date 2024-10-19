namespace Fasciculus.Eve.Models
{
    public class SdeUniverse
    {
        public SdeRegions Regions { get; }

        public SdeUniverse(SdeRegions regions)
        {
            Regions = regions;
        }

        public void Populate(SdeData data)
        {
            Regions.Populate(data);
        }
    }
}
