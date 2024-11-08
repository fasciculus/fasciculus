using Fasciculus.Eve.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Eve.Core.Tests
{
    [TestClass]
    public class EveNavigationTests : EveCoreTests
    {
        [TestMethod]
        public void TestAtRange()
        {
            EveResources.ReadNavigation(universe);
        }
    }
}
