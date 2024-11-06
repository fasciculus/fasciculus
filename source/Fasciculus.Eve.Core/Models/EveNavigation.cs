namespace Fasciculus.Eve.Models
{
    public class EveNavigation
    {
        private readonly EveDistances highSecDistances;
        private readonly EveDistances highAndLowSecDistances;
        private readonly EveDistances allDistances;

        private EveNavigation(EveDistances highSecDistances, EveDistances highAndLowSecDistances, EveDistances allDistances)
        {
            this.highSecDistances = highSecDistances;
            this.highAndLowSecDistances = highAndLowSecDistances;
            this.allDistances = allDistances;
        }

        public static EveNavigation Create(IEveUniverse universe)
        {
            EveConnections connections = EveConnections.Create(universe);
            EveDistances highSecDistances = EveDistancesFactory.Create(universe, connections, EveSecurity.High);
            EveDistances highAndLowSecDistances = EveDistancesFactory.Create(universe, connections, EveSecurity.LowAndHigh);
            EveDistances allDistances = EveDistancesFactory.Create(universe, connections, EveSecurity.All);

            return new(highSecDistances, highAndLowSecDistances, allDistances);
        }
    }
}
