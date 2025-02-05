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
            SvgRoot svg = SvgRoot.Create(0, 0, 160, 80).Width("16rem").Height("8rem").Build();

            SvgRect rect1 = SvgRect.Create(10, 10, 40, 20).Fill("red").StrokeWidth("10").Build();
            SvgRect rect2 = SvgRect.Create(30, 20, 100, 40).Fill("green").StrokeWidth("10").Build();
            SvgRect rect3 = SvgRect.Create(110, 50, 40, 20).Fill("blue").StrokeWidth("10").Build();

            svg.Add(rect1);
            svg.Add(rect2);
            svg.Add(rect3);

            FileInfo file = SpecialDirectories.WorkingDirectory.File("SvgTests.svg");

            file.WriteAllText(svg.ToString());
        }
    }
}
