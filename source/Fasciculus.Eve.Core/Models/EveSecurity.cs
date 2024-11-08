using System;
using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class EveSecurity
    {
        public int Index { get; }
        public Func<EveSolarSystem, bool> Filter { get; }

        public EveSecurity(int index, Func<EveSolarSystem, bool> filter)
        {
            Index = index;
            Filter = filter;
        }

        public static readonly EveSecurity All
            = new(0, (ss) => true);

        public static readonly EveSecurity LowAndHigh
            = new(0, (ss) => ss.Security >= 0.0);

        public static readonly EveSecurity High
            = new(0, (ss) => ss.Security >= 0.5);

        public static IEnumerable<EveSecurity> Levels
            => [All, LowAndHigh, High];

        public static string Format(double security)
        {
            security = Math.Floor(security * 10.0) / 10.0;

            return $"{security:0.0}";
        }
    }
}
