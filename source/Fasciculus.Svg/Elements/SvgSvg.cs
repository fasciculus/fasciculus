using Fasciculus.Svg.Builders;
using Fasciculus.Svg.Types;
using Fasciculus.Xml;
using System.Xml.Linq;

namespace Fasciculus.Svg.Elements
{
    /// <summary>
    /// Represents a <c>SVGSVGElement</c>
    /// </summary>
    public class SvgSvg : SvgElement
    {
        public static XAttribute NamespaceAttribute
            => new("xmlns", NamespaceUri);

        public SvgViewBox ViewBox
        {
            get => new(this.GetDoubles("viewBox"));
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

        public SvgSvg(SvgViewBox viewBox)
            : base(Namespace + "svg", NamespaceAttribute)
        {
            ViewBox = viewBox;
        }

        public SvgSvg(double minX, double minY, double width, double height)
            : this(new(minX, minY, width, height)) { }

        public SvgSvg(double width, double height)
            : this(0, 0, width, height) { }

        public static SvgSvgBuilder Create(SvgViewBox viewBox)
            => new(viewBox);

        public static SvgSvgBuilder Create(double minX, double minY, double width, double height)
            => Create(new(minX, minY, width, height));

        public static SvgSvgBuilder Create(double width, double height)
            => Create(0, 0, width, height);
    }
}
