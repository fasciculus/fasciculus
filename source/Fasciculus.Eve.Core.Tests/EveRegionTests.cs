using Fasciculus.Eve.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fasciculus.Eve.Core.Tests
{
    [TestClass]
    public class EveRegionTests : EveCoreTests
    {
        [TestMethod]
        public void TestNeighbours()
        {
            EveRegion theForge = universe.Regions["The Forge"];
            IEnumerable<EveRegion> neighbours = theForge.GetNeighbours(EveSecurity.All);
            string[] names = neighbours.Select(x => x.Name).ToArray();

            Assert.IsTrue(names.Contains("The Citadel"));

            names.Apply(n => Debug.WriteLine(n));
        }
    }
}
