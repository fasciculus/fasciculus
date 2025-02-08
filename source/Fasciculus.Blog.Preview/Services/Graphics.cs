using Fasciculus.Markdown.Extensions.Svg;
using System.Net.Http;
using System.Xml.Linq;

namespace Fasciculus.Blog.Preview.Services
{
    public class Graphics : Client, ISvgMappings
    {
        public Graphics(HttpClient client)
            : base(client)
        {
        }

        public XElement? GetSvg(string key)
        {
            string text = GetString($"Graphic/{key}");

            return string.IsNullOrEmpty(text) ? null : XElement.Parse(text);
        }
    }
}
