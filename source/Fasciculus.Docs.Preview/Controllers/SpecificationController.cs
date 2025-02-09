using Fasciculus.Docs.Preview.Models;
using Fasciculus.Docs.Preview.Services;
using Fasciculus.IO;
using Markdig;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Docs.Preview.Controllers
{
    public class SpecificationController : Controller
    {
        private readonly SpecificationClient specifications;
        private readonly MarkdownPipeline pipeline;

        public SpecificationController(SpecificationClient specifications, MarkdownPipeline pipeline)
        {
            this.specifications = specifications;
            this.pipeline = pipeline;
        }

        [Route("/Specifications/")]
        public IActionResult Index()
        {
            SpecificationIndexViewModel model = new()
            {
                Title = "Specifications",
                Version = specifications.GetVersion(),
                Keys = specifications.GetKeys(),
            };

            return View("Index", model);
        }

        [Route("/Specifications/{key}")]
        public IActionResult Entry(string key)
        {
            string text = specifications.GetFile(key).ReadAllText();

            SpecificationEntryViewModel model = new()
            {
                Title = key,
                Version = specifications.GetVersion(),
                Html = Markdig.Markdown.ToHtml(text, pipeline)
            };

            return View("Entry", model);
        }
    }
}
