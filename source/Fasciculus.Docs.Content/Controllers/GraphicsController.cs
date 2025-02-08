using Fasciculus.Docs.Content.Services;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace Fasciculus.Docs.Content.Controllers
{
    public class GraphicsController : Controller
    {
        private readonly GraphicsProvider graphics;

        public GraphicsController(GraphicsProvider graphics)
        {
            this.graphics = graphics;
        }

        [Route("/Graphic/{key}")]
        public string Graphic(string key)
        {
            XElement? graphic = graphics.GetSvg(key);

            return graphic is null ? string.Empty : graphic.ToString();
        }
    }
}
