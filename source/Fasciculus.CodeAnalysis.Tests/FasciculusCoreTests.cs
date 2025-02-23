using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.CodeAnalysis.Tests
{
    [TestClass]
    public class FasciculusCoreTests : CodeAnalyzerTests
    {
        [TestMethod]
        public void Test()
        {
            CodeAnalyzerTestContext context = new()
            {
                Projects = [GetProject("Fasciculus.Core")],
                ProductionKind = SyntaxKind.OperatorDeclaration, // SyntaxKind.None,

                Packages = 1,
                Namespaces = 10,
                Enums = 0,
                Interfaces = 2,
                Classes = 34,

                Fields = 2,
                Members = 0,
                Events = 2,
                Properties = 30,

                Constructors = 28,
                Methods = 120,

                Summaries = 213,
            };

            Test(context);
        }
    }
}
