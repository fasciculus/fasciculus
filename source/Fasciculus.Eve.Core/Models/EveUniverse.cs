using Fasciculus.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveUniverse
    {
        public EveRegions Regions { get; }
        public EveConstellations Constellations { get; }
        public EveSolarSystems SolarSystems { get; }

        public EveUniverse(EveRegions regions)
        {
            Regions = regions;
            Constellations = new(Regions.SelectMany(r => r.Constellations));
            SolarSystems = new(Constellations.SelectMany(c => c.SolarSystems));
        }

        public void Write(Data data)
        {
            Regions.Write(data);
        }

        public static EveUniverse Read(Data data)
        {
            EveRegions regions = EveRegions.Read(data);

            return new(regions);
        }
    }
}
