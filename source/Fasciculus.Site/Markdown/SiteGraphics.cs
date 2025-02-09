using Fasciculus.Docs.Content.Services;
using Fasciculus.Markdown.Svg;
using System.Xml.Linq;

namespace Fasciculus.Site.Markdown
{
    public class SiteGraphics : ISvgMappings
    {
        private readonly ContentGraphics contentGraphics;

        public SiteGraphics(ContentGraphics contentGraphics)
        {
            this.contentGraphics = contentGraphics;
        }

        public XElement? GetSvg(string key)
        {
            return contentGraphics.GetSvg(key);
        }
    }
}
