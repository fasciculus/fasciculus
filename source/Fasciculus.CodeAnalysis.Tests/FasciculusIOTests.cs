using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.CodeAnalysis.Tests
{
    [TestClass]
    public class FasciculusIOTests : CodeAnalyzerTests
    {
        [TestMethod]
        public void Test()
        {
            CodeAnalyzerTestContext context = new()
            {
                Projects = [GetProject("Fasciculus.IO")],
                ProductionKind = SyntaxKind.None,

                Packages = 1,
                Namespaces = 3,
                Enums = 1,
                Interfaces = 0,
                Classes = 12,

                Fields = 0,
                Members = 3,
                Events = 0,
                Properties = 17,

                Constructors = 2,
                Methods = 168,

                Summaries = 203,
            };

            Test(context);
        }
    }
}
