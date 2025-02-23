using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.CodeAnalysis.Tests
{
    [TestClass]
    public class FasciculusTextTests : CodeAnalyzerTests
    {
        [TestMethod]
        public void Test()
        {
            CodeAnalyzerTestContext context = new()
            {
                Projects = [GetProject("Fasciculus.Text")],
                ProductionKind = SyntaxKind.None,

                Packages = 1,
                Namespaces = 1,
                Enums = 0,
                Interfaces = 0,
                Classes = 3,

                Fields = 0,
                Members = 0,
                Events = 0,
                Properties = 0,

                Constructors = 0,
                Methods = 4,

                Summaries = 7,
            };

            Test(context);
        }
    }
}
