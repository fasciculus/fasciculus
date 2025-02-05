using Fasciculus.IO;
using Fasciculus.Svg.Elements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Fasciculus.Svg.Tests
{
    [TestClass]
    public class SvgTests
    {
        [TestMethod]
        public void Test()
        {
            SvgRoot svg = SvgRoot.Create(0, 0, 160, 80)
                .Width("16rem").Height("8rem")
                .Build();

            SvgRect rect = SvgRect.Create(10, 10, 20, 10)
                .Fill("red")
                .Build();

            svg.Add(rect);

            FileInfo file = SpecialDirectories.WorkingDirectory.File("SvgTests.svg");

            file.WriteAllText(svg.ToString());
        }
    }
}
