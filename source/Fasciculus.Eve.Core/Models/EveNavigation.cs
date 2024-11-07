﻿using Fasciculus.IO;

namespace Fasciculus.Eve.Models
{
    public class EveNavigation
    {
        private readonly EveDistances[] solarSystemDistances;

        public EveNavigation(EveDistances[] solarSystemDistances)
        {
            this.solarSystemDistances = solarSystemDistances;
        }

        public void Write(Data data)
        {
            data.WriteArray(solarSystemDistances, d => d.Write(data));
        }
    }
}
