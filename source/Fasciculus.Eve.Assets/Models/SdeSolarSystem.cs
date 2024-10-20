namespace Fasciculus.Eve.Models
{
    public class SdeSolarSystem
    {
        public int SolarSystemID { get; set; }
        public double Security { get; set; }
        public string SecurityClass { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public void Populate(SdeData data)
        {
            Name = data.Names[SolarSystemID];
        }
    }
}
