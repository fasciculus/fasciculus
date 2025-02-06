using Fasciculus.Svg.Elements;
using Fasciculus.Svg.Types;

namespace Fasciculus.Svg.Builders
{
    public class SvgSvgBuilder : SvgElementBuilder<SvgSvgBuilder, SvgSvg>
    {
        public SvgSvgBuilder(SvgViewBox viewBox)
            : base(() => new SvgSvg(viewBox)) { }

        public SvgSvgBuilder ViewBox(SvgViewBox viewBox)
        {
            element.ViewBox = viewBox;

            return this;
        }

        public SvgSvgBuilder Width(string width)
        {
            element.Width = width;

            return this;
        }

        public SvgSvgBuilder Height(string height)
        {
            element.Height = height;

            return this;
        }

        protected override SvgSvgBuilder This()
            => this;
    }
}
