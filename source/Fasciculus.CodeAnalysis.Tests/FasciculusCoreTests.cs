using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.CodeAnalysis.Tests
{
    [TestClass]
    public class FasciculusCoreTests : CodeAnalyzerTests
    {
        [TestMethod]
        public void Test()
        {
            Test(CodeAnalyzerTestContexts.FasciculusCore);
        }
    }
}
