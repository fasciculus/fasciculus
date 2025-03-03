using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.CodeAnalysis.Tests
{
    [TestClass]
    public class FasciculusThreadingCommonTests : CodeAnalyzerTests
    {
        [TestMethod]
        public void Test()
        {
            Test(CodeAnalyzerTestContexts.FasciculusThreadingCommon);
        }
    }
}
