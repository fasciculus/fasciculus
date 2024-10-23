namespace Fasciculus.Eve.Models
{
    public interface IEveUniverse
    {
        public EveRegions Regions { get; }
        public EveSolarSystems SolarSystems { get; }
        public EveStargates Stargates { get; }
    }
}
