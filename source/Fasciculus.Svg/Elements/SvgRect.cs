using Fasciculus.Svg.Builders;
using Fasciculus.Xml;

namespace Fasciculus.Svg.Elements
{
    public class SvgRect : SvgShapeElement
    {
        public double X
        {
            get => this.GetDouble("x");
            set => this.SetDouble("x", value);
        }

        public double Y
        {
            get => this.GetDouble("y");
            set => this.SetDouble("y", value);
        }

        public double Width
        {
            get => this.GetDouble("width");
            set => this.SetDouble("width", value);
        }

        public double Height
        {
            get => this.GetDouble("height");
            set => this.SetDouble("height", value);
        }

        public double RX
        {
            get => this.GetDouble("rx");
            set => this.SetDouble("rx", value);
        }

        public double RY
        {
            get => this.GetDouble("ry");
            set => this.SetDouble("ry", value);
        }

        public SvgRect(double x, double y, double width, double height)
            : base("rect")
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public SvgRect()
            : this(0, 0, 0, 0) { }

        public static SvgRectBuilder Create()
            => new(0, 0, 0, 0);

        public static SvgRectBuilder Create(double x, double y, double width, double height)
            => new(x, y, width, height);
    }
}
