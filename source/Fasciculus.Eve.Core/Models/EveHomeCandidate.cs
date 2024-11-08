namespace Fasciculus.Eve.Models
{
    public class EveHomeCandidate
    {
        public EveSolarSystem SolarSystem { get; }

        public short DistanceToJita { get; private set; }

        public double Rating { get; private set; }

        private EveHomeCandidate(EveSolarSystem solarSystem)
        {
            SolarSystem = solarSystem;
        }

        public static EveHomeCandidate Create(EveSolarSystem solarSystem, IEveUniverse universe, EveNavigation navigation)
        {
            EveHomeCandidate candidate = new(solarSystem);
            EveSolarSystem jita = universe.SolarSystems["Jita"];

            candidate.DistanceToJita = navigation.GetDistance(solarSystem, jita, EveSecurity.High);
            candidate.Rating = Rate(candidate);

            return candidate;
        }

        private static double Rate(EveHomeCandidate candidate)
        {
            return 10 - candidate.DistanceToJita;
        }

        public override string? ToString()
        {
            return $"{SolarSystem} | Jita: {DistanceToJita}";
        }
    }
}
