using Fasciculus.Svg.Elements;

namespace Fasciculus.Svg.Builders
{
    public class SvgRectBuilder : SvgShapeBuilder<SvgRectBuilder, SvgRect>
    {
        public SvgRectBuilder(double x, double y, double width, double height)
        {
            X(x);
            Y(y);
            Width(width);
            Height(height);
        }

        public SvgRectBuilder X(double x)
        {
            element.X = x;

            return this;
        }

        public SvgRectBuilder Y(double y)
        {
            element.Y = y;

            return this;
        }

        public SvgRectBuilder Width(double width)
        {
            element.Width = width;

            return this;
        }

        public SvgRectBuilder Height(double height)
        {
            element.Height = height;

            return this;
        }

        public SvgRectBuilder XY(double x, double y)
            => X(x).Y(y);

        public SvgRectBuilder WH(double width, double height)
            => Width(width).Height(height);

        public SvgRectBuilder XYWH(double x, double y, double width, double height)
            => X(x).Y(y).Width(width).Height(height);

        public SvgRectBuilder RX(double rx)
        {
            element.RX = rx;

            return this;
        }

        public SvgRectBuilder RY(double ry)
        {
            element.RY = ry;

            return this;
        }

        public SvgRectBuilder R(double rx, double ry)
            => RX(rx).RY(ry);

        protected override SvgRectBuilder GetThis()
            => this;
    }
}
