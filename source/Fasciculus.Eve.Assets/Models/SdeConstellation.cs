namespace Fasciculus.Eve.Models
{
    public class SdeConstellation
    {
        public int ConstellationID { get; set; }

        public string Name { get; set; } = string.Empty;

        public void Populate(SdeData data)
        {
            Name = data.Names[ConstellationID];
        }
    }
}
