using Fasciculus.Eve.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fasciculus.Eve.Core.Tests
{
    [TestClass]
    public class EveHomeCandidateTests : EveCoreTests
    {
        [TestMethod]
        public void Test()
        {
            EveSolarSystem jita = universe.SolarSystems["Jita"];

            IEnumerable<EveSolarSystem> solarSystens = Enumerable.Range(3, 3)
                .SelectMany(distance => navigation.AtRange(jita, distance, EveSecurity.High));

            IEnumerable<EveHomeCandidate> candidates = solarSystens
                .Select(ss => EveHomeCandidate.Create(ss, universe, navigation))
                .OrderBy(c => c.SolarSystem.Index)
                .OrderBy(c => c.Rating)
                .Reverse();

            candidates.Take(3).Apply(c => Debug.WriteLine(c));
        }
    }
}
