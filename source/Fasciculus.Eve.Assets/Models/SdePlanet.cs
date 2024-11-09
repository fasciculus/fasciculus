using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class SdePlanet
    {
        public int TypeId { get; set; }
        public Dictionary<int, SdeAsteroidBelt> AsteroidBelts { get; set; } = [];
        public Dictionary<int, SdeMoon> Moons { get; set; } = [];
    }
}
