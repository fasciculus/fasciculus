using Fasciculus.IO;

namespace Fasciculus.Eve.Models
{
    public class EveUniverse
    {
        public EveRegions Regions { get; }

        public EveUniverse(EveRegions regions)
        {
            Regions = regions;
        }

        public void Write(Data data)
        {
            Regions.Write(data);
        }
    }
}
