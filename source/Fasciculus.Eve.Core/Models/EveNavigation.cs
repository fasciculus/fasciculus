namespace Fasciculus.Eve.Models
{
    public class EveNavigation
    {
        private readonly EveDistances[] solarSystemDistances;

        public EveNavigation(EveDistances[] solarSystemDistances)
        {
            this.solarSystemDistances = solarSystemDistances;
        }
    }
}
