using Fasciculus.IO;
using System.IO;
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

        public void Write(Stream stream)
        {
            Data data = stream;

            Regions.Write(data);
        }

        public static EveUniverse Read(Stream stream)
        {
            Data data = stream;

            EveRegions regions = EveRegions.Read(data);

            return new(regions);
        }
    }
}
