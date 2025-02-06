using System.Xml.Linq;

namespace Fasciculus.Markdown.Extensions.Svg
{
    public interface ISvgMappings
    {
        public XElement? GetSvg(string key);
    }
}
