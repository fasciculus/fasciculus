using Fasciculus.Svg.Attributes;
using Fasciculus.Svg.Elements;
using System.Xml.Linq;

namespace Fasciculus.Blog.Y2025.M02
{
    public static class Test
    {
        [Svg("Test1")]
        public static XElement Test1()
        {
            SvgSvg svg = SvgSvg.Create(0, 0, 160, 80);

            SvgRect rect1 = SvgRect.Create(10, 10, 40, 20).Fill("red").Stroke("black").StrokeWidth("2");
            SvgRect rect2 = SvgRect.Create(30, 20, 100, 40).Fill("green").Stroke("black").StrokeWidth("2");
            SvgRect rect3 = SvgRect.Create(110, 50, 40, 20).Fill("blue").Stroke("black").StrokeWidth("2");

            svg.Add(rect1);
            svg.Add(rect2);
            svg.Add(rect3);

            return svg;
        }
    }
}
