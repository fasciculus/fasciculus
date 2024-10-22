using Fasciculus.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveUniverse
    {
        public EveRegions Regions { get; }
        public EveConstellations Constellations { get; }

        public EveUniverse(EveRegions regions)
        {
            Regions = regions;
            Constellations = new(Regions.SelectMany(r => r.Constellations));
        }

        public void Write(Data data)
        {
            Regions.Write(data);
        }
    }
}
