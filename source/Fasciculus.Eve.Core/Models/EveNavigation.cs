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
            EveDistances highSecDistances = EveDistances.Create(universe, 0.5);
            EveDistances highAndLowSecDistances = EveDistances.Create(universe, 0.0);
            EveDistances allDistances = EveDistances.Create(universe, -10.0);

            return new(highSecDistances, highAndLowSecDistances, allDistances);
        }
    }
}
