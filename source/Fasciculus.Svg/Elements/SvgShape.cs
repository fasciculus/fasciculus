using Fasciculus.Xml;
using System.Xml.Linq;

namespace Fasciculus.Svg.Elements
{
    public abstract class SvgShape : SvgElement
    {
        public string? Fill
        {
            get => this.HasAttribute("fill") ? this.GetString("fill") : null;
            set => SetAttributeValue("fill", value);
        }

        public string? Stroke
        {
            get => this.HasAttribute("stroke") ? this.GetString("stroke") : null;
            set => SetAttributeValue("stroke", value);
        }

        public string? StrokeWidth
        {
            get => this.HasAttribute("stroke-width") ? this.GetString("stroke-width") : null;
            set => SetAttributeValue("stroke-width", value);
        }

        protected SvgShape(XName name)
            : base(name) { }
    }
}
