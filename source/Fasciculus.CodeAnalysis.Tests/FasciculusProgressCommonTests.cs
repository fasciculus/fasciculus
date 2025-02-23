using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.CodeAnalysis.Tests
{
    [TestClass]
    public class FasciculusProgressCommonTests : CodeAnalyzerTests
    {
        [TestMethod]
        public void Test()
        {
            CodeAnalyzerTestContext context = new()
            {
                Projects = [GetProject("Fasciculus.Progress.Common")],
                ProductionKind = SyntaxKind.None,

                Packages = 1,
                Namespaces = 1,
                Enums = 0,
                Interfaces = 1,
                Classes = 0,

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
