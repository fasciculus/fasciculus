using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.CodeAnalysis.Tests
{
    [TestClass]
    public class FasciculusAlgorithmsTests : CodeAnalyzerTests
    {
        [TestMethod]
        public void Test()
        {
            CodeAnalyzerTestContext context = new()
            {
                Projects = [GetProject("Fasciculus.Algorithms")],
                ProductionKind = SyntaxKind.Parameter,

                Packages = 1,
                Namespaces = 4,
                Enums = 0,
                Interfaces = 0,
                Classes = 7,

                Fields = 0,
                Members = 0,
                Events = 0,
                Properties = 4,

                Constructors = 1,
                Methods = 17,

                Summaries = 29,
            };

            Test(context);
        }
    }
}
