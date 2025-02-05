using Fasciculus.Svg.Builders;
using Fasciculus.Svg.Types;
using Fasciculus.Xml;

namespace Fasciculus.Svg.Elements
{
    /// <summary>
    /// Represents a <c>SVGSVGElement</c>
    /// </summary>
    public class SvgRoot : SvgElement
    {
        public SvgViewBox? ViewBox
        {
            get => this.HasAttribute("viewBox") ? new(this.GetDoubles("viewBox")) : null;
            set { if (value is null) SetAttributeValue("viewBox", null); else this.SetDoubles("viewBox", value); }
        }

        public string Width
        {
            get => this.GetString("width", "auto");
            set => SetAttributeValue("width", value);
        }

        public string Height
        {
            get => this.GetString("height", "auto");
            set => SetAttributeValue("height", value);
        }

        public SvgRoot()
            : base("svg") { }

        public static SvgRootBuilder Create()
            => new();

        public static SvgRootBuilder Create(SvgViewBox viewBox)
            => new(viewBox);

        public static SvgRootBuilder Create(double minX, double minY, double width, double height)
            => new(minX, minY, width, height);
    }
}
