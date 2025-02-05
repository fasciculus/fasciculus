using System.Xml.Linq;

namespace Fasciculus.Svg.Elements
{
    /// <summary>
    /// Represents a <c>SVGSVGElement</c>
    /// </summary>
    public class SvgRoot : XElement
    {
        public SvgRoot()
            : base("svg")
        {
        }
    }
}
