using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.CodeAnalysis.Tests
{
    [TestClass]
    public class FasciculusAlgorithmsTests : CodeAnalyzerTests
    {
        [TestMethod]
        public void TestFasciculusAlgorithms()
        {
            Test(CodeAnalyzerTestContexts.FasciculusAlgorithms);
        }
    }
}
