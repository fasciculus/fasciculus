using Fasciculus.Validating;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveHomeCandidate
    {
        internal EveSolarSystem? tradeHub;
        internal EveSolarSystem? danger;
        internal EveSolarSystem? ice;

        public EveSolarSystem SolarSystem { get; }

        public EveSolarSystem TradeHub => Cond.NotNull(tradeHub);
        public short TradeHubDistance { get; internal set; }

        public EveSolarSystem Danger => Cond.NotNull(danger);
        public short DangerDistance { get; internal set; }

        public EveSolarSystem Ice => Cond.NotNull(ice);
        public short IceDistance { get; internal set; }

        public int AsteroidBelts { get; internal set; }

        public int Rating { get; internal set; }

        public EveHomeCandidate(EveSolarSystem solarSystem)
        {
            SolarSystem = solarSystem;
        }

        public override string? ToString()
        {
            return $"{SolarSystem} = {Rating}";
        }
    }

    public class EveHomeRating
    {
        private readonly IEveUniverse universe;
        private readonly EveNavigation navigation;

        public EveSolarSystem TradeHub { get; }
        public short TradeHubMinDistance { get; set; } = 3;
        public short TradeHubMaxDistance { get; set; } = 5;
        public int TradeHubDistancePenalty { get; set; } = 20;

        public double DesiredSecurity { get; set; } = 0.6;
        public int DesiredSecurityPenalty { get; set; } = 200;

        public int DangerDistanceReward { get; set; } = 5;
        public int IceDistancePenalty { get; set; } = 20;
        public int AsteroidBeltReward { get; set; } = 1;

        public EveHomeRating(IEveUniverse universe, EveNavigation navigation)
        {
            this.universe = universe;
            this.navigation = navigation;

            TradeHub = universe.SolarSystems["Jita"];
        }

        public IEnumerable<EveHomeCandidate> Find(int count)
        {
            return GetSolarSystemsInTradeHubRange()
                .Select(CreateCandidate)
                .OrderBy(c => c.Rating)
                .Reverse()
                .Take(count);
        }

        private IEnumerable<EveSolarSystem> GetSolarSystemsInTradeHubRange()
        {
            return Enumerable.Range(TradeHubMinDistance, TradeHubMaxDistance - TradeHubMinDistance + 1)
                 .SelectMany(distance => navigation.AtRange(TradeHub, distance, EveSecurity.High));
        }

        private EveHomeCandidate CreateCandidate(EveSolarSystem solarSystem)
        {
            EveHomeCandidate candidate = new(solarSystem);

            candidate.tradeHub = TradeHub;
            candidate.TradeHubDistance = navigation.GetDistance(solarSystem, TradeHub, EveSecurity.High);

            candidate.danger = navigation.Nearest(solarSystem, EveSecurity.All, ss => ss.Security < 0.5);
            candidate.DangerDistance = navigation.GetDistance(solarSystem, candidate.Danger, EveSecurity.All);

            candidate.ice = navigation.Nearest(solarSystem, EveSecurity.All, ss => ss.HasIce);
            candidate.IceDistance = navigation.GetDistance(solarSystem, candidate.Ice, EveSecurity.All);

            candidate.AsteroidBelts = solarSystem.Planets.SelectMany(p => p.Moons).Count();

            candidate.Rating = Rate(candidate);

            return candidate;
        }

        private int Rate(EveHomeCandidate candidate)
        {
            int rating = 100;

            rating -= (candidate.TradeHubDistance - TradeHubMinDistance) * TradeHubDistancePenalty;
            rating -= (int)Math.Round(Math.Abs(candidate.SolarSystem.Security - DesiredSecurity) * DesiredSecurityPenalty);
            rating += candidate.DangerDistance * DangerDistanceReward;
            rating -= candidate.IceDistance * IceDistancePenalty;
            rating += candidate.AsteroidBelts * AsteroidBeltReward;

            return rating;
        }
    }
}
