using Fasciculus.Eve.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Eve.Core.Tests
{
    [TestClass]
    public class EveNavigationTests : EveCoreTests
    {
        [TestMethod]
        public void TestAtRange()
        {
            EveNavigation navigation = EveNavigation.Create(universe);
        }
    }
}
