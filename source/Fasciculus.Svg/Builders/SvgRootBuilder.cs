using Fasciculus.Svg.Elements;
using Fasciculus.Svg.Types;

namespace Fasciculus.Svg.Builders
{
    public class SvgRootBuilder : SvgElementBuilder<SvgRootBuilder, SvgRoot>
    {
        public SvgRootBuilder() { }

        public SvgRootBuilder(SvgViewBox viewBox)
        {
            ViewBox(viewBox);
        }

        public SvgRootBuilder(double minX, double minY, double width, double height)
        {
            ViewBox(minX, minY, width, height);
        }

        public SvgRootBuilder ViewBox(SvgViewBox viewBox)
        {
            element.ViewBox = viewBox;

            return this;
        }

        public SvgRootBuilder ViewBox(double minX, double minY, double width, double height)
        {
            element.ViewBox = new(minX, minY, width, height);

            return this;
        }

        public SvgRootBuilder Width(string width)
        {
            element.Width = width;

            return this;
        }

        public SvgRootBuilder Height(string height)
        {
            element.Height = height;

            return this;
        }

        protected override SvgRootBuilder GetThis()
            => this;
    }
}
