using Fasciculus.IO;
using Fasciculus.Svg.Elements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Fasciculus.Svg.Tests
{
    [TestClass]
    public class SvgTests : TestsBase
    {
        [TestMethod]
        public void Test()
        {
            SvgSvg svg = SvgSvg.Create(0, 0, 160, 80).Width("16rem").Height("8rem");

            SvgRect rect1 = SvgRect.Create(10, 10, 40, 20).Fill("red").Stroke("black").StrokeWidth("2");
            SvgRect rect2 = SvgRect.Create(30, 20, 100, 40).Fill("green").Stroke("black").StrokeWidth("2");
            SvgRect rect3 = SvgRect.Create(110, 50, 40, 20).Fill("blue").Stroke("black").StrokeWidth("2");

            svg.Add(rect1);
            svg.Add(rect2);
            svg.Add(rect3);

            string asText = svg.ToString();

            Log(asText);

            FileInfo file = SpecialDirectories.WorkingDirectory.File("SvgTests.svg");

            file.WriteAllText(asText);
        }
    }
}
