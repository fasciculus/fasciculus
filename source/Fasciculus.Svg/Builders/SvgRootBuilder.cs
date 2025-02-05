using Fasciculus.Svg.Elements;
using Fasciculus.Svg.Types;

namespace Fasciculus.Svg.Builders
{
    public class SvgRootBuilder : SvgElementBuilder<SvgRootBuilder, SvgSvg>
    {
        public SvgRootBuilder(SvgViewBox viewBox)
            : base(() => new SvgSvg(viewBox)) { }

        public SvgRootBuilder ViewBox(SvgViewBox viewBox)
        {
            element.ViewBox = viewBox;

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

        protected override SvgRootBuilder This()
            => this;
    }
}
