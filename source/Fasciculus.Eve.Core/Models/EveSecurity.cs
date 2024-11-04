using System;
using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class EveSecurityLevel
    {
        public int Index { get; }
        public Func<EveSolarSystem, bool> Filter { get; }

        public EveSecurityLevel(int index, Func<EveSolarSystem, bool> filter)
        {
            Index = index;
            Filter = filter;
        }

        public static readonly EveSecurityLevel All
            = new(0, (ss) => true);

        public static readonly EveSecurityLevel LowAndHigh
            = new(0, (ss) => ss.Security >= 0.0);

        public static readonly EveSecurityLevel High
            = new(0, (ss) => ss.Security >= 0.5);

        public static IEnumerable<EveSecurityLevel> Levels
            => [All, LowAndHigh, High];
    }
}
