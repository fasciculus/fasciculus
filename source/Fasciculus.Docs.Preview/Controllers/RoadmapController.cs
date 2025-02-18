using Fasciculus.Docs.Preview.Models;
using Fasciculus.Docs.Preview.Services;
using Fasciculus.IO;
using Markdig;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Docs.Preview.Controllers
{
    public class RoadmapController : Controller
    {
        private readonly RoadmapClient roadmap;
        private readonly MarkdownPipeline pipeline;

        public RoadmapController(RoadmapClient roadmap, MarkdownPipeline pipeline)
        {
            this.roadmap = roadmap;
            this.pipeline = pipeline;
        }

        [Route("/Roadmap.html")]
        public IActionResult Roadmap()
        {
            string text = roadmap.GetFile("Roadmap").ReadAllText();

            RoadmapViewModel model = new()
            {
                Title = "Roadmap",
                Version = roadmap.GetVersion(),
                Html = Markdig.Markdown.ToHtml(text, pipeline)
            };

            return View("Roadmap", model);
        }
    }
}
