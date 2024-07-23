using System.Collections.Generic;

namespace Fasciculus.Eve.Models.Sde
{

#pragma warning disable IDE1006 // Naming Styles

    public class SdeSolarSystem
    {
        public int solarSystemID { get; set; }
        public double security { get; set; }
        public string securityClass { get; set; } = string.Empty;

        public Dictionary<int, SdeStargate> stargates { get; set; } = new();
    }

#pragma warning restore IDE1006 // Naming Styles

}
