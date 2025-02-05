using Fasciculus.Xml;

namespace Fasciculus.Svg.Elements
{
    public abstract class SvgShapeElement : SvgElement
    {
        public string? Fill
        {
            get => this.HasAttribute("fill") ? this.GetString("fill") : null;
            set => SetAttributeValue("fill", value);
        }

        protected SvgShapeElement(string name)
            : base(name) { }
    }
}
