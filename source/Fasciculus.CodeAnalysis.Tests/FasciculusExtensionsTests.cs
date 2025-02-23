using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.CodeAnalysis.Tests
{
    [TestClass]
    public class FasciculusExtensionsTests : CodeAnalyzerTests
    {
        [TestMethod]
        public void Test()
        {
            CodeAnalyzerTestContext context = new()
            {
                Projects = [GetProject("Fasciculus.Extensions")],
                ProductionKind = SyntaxKind.None,

                Packages = 1,
                Namespaces = 2,
                Enums = 0,
                Interfaces = 0,
                Classes = 4,

                Fields = 0,
                Members = 0,
                Events = 0,
                Properties = 2,

                Constructors = 2,
                Methods = 7,

                Summaries = 13,
            };

            Test(context);
        }
    }
}
