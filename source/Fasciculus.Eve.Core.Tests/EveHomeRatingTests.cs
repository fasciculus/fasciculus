using Fasciculus.Eve.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
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

            candidates.Apply(Log);
        }

        private static void Log(EveHomeCandidate candidate)
        {
            Debug.WriteLine(candidate);
            Debug.WriteLine($"  TradeHub = {candidate.TradeHub} @ {candidate.TradeHubDistance} jumps");
            Debug.WriteLine($"  Danger   = {candidate.Danger} @ {candidate.DangerDistance} jumps");
            Debug.WriteLine($"  Ice      = {candidate.Ice} @ {candidate.IceDistance} jumps");
        }
    }
}
