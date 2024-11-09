using Fasciculus.Eve.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Core.Tests
{
    [TestClass]
    public class EveHomeRatingTests : EveCoreTests
    {
        [TestMethod]
        public void Test()
        {
            EveHomeRating homeRating = new(universe, navigation);
            EveHomeCandidate[] candidates = homeRating.Find(3).ToArray();

            candidates.Apply(LogCandidate);
        }

        private void LogCandidate(EveHomeCandidate candidate)
        {
            Log(candidate.ToString());
            Log($"  TradeHub  = {candidate.TradeHub} @ {candidate.TradeHubDistance} jumps");
            Log($"  Danger    = {candidate.Danger} @ {candidate.DangerDistance} jumps");
            Log($"  Ice       = {candidate.Ice} @ {candidate.IceDistance} jumps");
            Log($"  Asteroids = {candidate.AsteroidBelts}");
        }
    }
}
