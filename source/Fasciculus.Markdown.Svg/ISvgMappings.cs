using System.Xml.Linq;

namespace Fasciculus.Markdown.Svg
{
    public interface ISvgMappings
    {
        public XElement? GetSvg(string key);
    }
}
