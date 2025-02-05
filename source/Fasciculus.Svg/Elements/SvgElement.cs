using Fasciculus.Xml;
using System.Xml.Linq;

namespace Fasciculus.Svg.Elements
{
    public abstract class SvgElement : XElement
    {
        public const string NamespaceUri = "http://www.w3.org/2000/svg";

        public static XNamespace Namespace => NamespaceUri;

        public string? Class
        {
            get => this.HasAttribute("class") ? this.GetString("class") : null;
            set => SetAttributeValue("class", value);
        }

        public string? Style
        {
            get => this.HasAttribute("style") ? this.GetString("style") : null;
            set => SetAttributeValue("style", value);
        }

        public string? Color
        {
            get => this.HasAttribute("color") ? this.GetString("color") : null;
            set => SetAttributeValue("color", value);
        }

        public string? FontFamily
        {
            get => this.HasAttribute("font-family") ? this.GetString("font-family") : null;
            set => SetAttributeValue("font-family", value);
        }

        public string? FontSize
        {
            get => this.HasAttribute("font-size") ? this.GetString("font-size") : null;
            set => SetAttributeValue("font-size", value);
        }

        protected SvgElement(XName name, params object[] content)
            : base(name, content) { }
    }
}
