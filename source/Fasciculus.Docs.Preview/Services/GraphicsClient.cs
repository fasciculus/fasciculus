using Fasciculus.Markdown.Extensions.Svg;
using System.Net.Http;
using System.Xml.Linq;

namespace Fasciculus.Docs.Preview.Services
{
    public class GraphicsClient : ContentClient, ISvgMappings
    {
        public GraphicsClient(HttpClient httpClient)
            : base(httpClient)
        {
        }

        public XElement? GetSvg(string key)
        {
            string text = GetString($"Graphic/{key}");

            return string.IsNullOrEmpty(text) ? null : XElement.Parse(text);
        }
    }
}
