using Fasciculus.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveUniverse : IEveUniverse
    {
        public EveRegions Regions { get; }
        public EveConstellations Constellations { get; }
        public EveSolarSystems SolarSystems { get; }
        public EveStargates Stargates { get; }

        public EveUniverse(EveRegions regions)
        {
            Regions = regions;
            Constellations = new(Regions.SelectMany(r => r.Constellations));
            SolarSystems = new(Constellations.SelectMany(c => c.SolarSystems));
            Stargates = new(SolarSystems.SelectMany(s => s.Stargates));

            Regions.Link(this);
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
