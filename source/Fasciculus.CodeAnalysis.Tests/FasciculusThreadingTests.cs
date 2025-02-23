using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.CodeAnalysis.Tests
{
    [TestClass]
    public class FasciculusThreadingTests : CodeAnalyzerTests
    {
        [TestMethod]
        public void Test()
        {
            CodeAnalyzerTestContext context = new()
            {
                Projects = [GetProject("Fasciculus.Threading")],
                ProductionKind = SyntaxKind.None,

                Packages = 1,
                Namespaces = 1,
                Enums = 0,
                Interfaces = 0,
                Classes = 1,

                Fields = 0,
                Members = 0,
                Events = 0,
                Properties = 0,

                Constructors = 0,
                Methods = 2,

                Summaries = 3,
            };

            Test(context);
        }
    }
}
