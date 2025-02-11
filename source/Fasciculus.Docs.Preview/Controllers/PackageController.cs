using Fasciculus.Docs.Preview.Models;
using Fasciculus.Docs.Preview.Services;
using Fasciculus.IO;
using Fasciculus.Markdown.Yaml;
using Fasciculus.Yaml;
using Markdig;
using Markdig.Syntax;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Docs.Preview.Controllers
{
    public class PackageController : Controller
    {
        private readonly PackageClient packages;
        private readonly MarkdownPipeline pipeline;

        public PackageController(PackageClient packages, MarkdownPipeline pipeline)
        {
            this.packages = packages;
            this.pipeline = pipeline;
        }

        [Route("/Packages/")]
        public IActionResult Index()
        {
            PackageIndexViewModel model = new()
            {
                Title = "Packages",
                Version = packages.GetVersion(),
                Keys = packages.GetKeys(),
            };

            return View("Index", model);
        }

        [Route("/Packages/{key}")]
        public IActionResult Entry(string key)
        {
            string text = packages.GetFile(key).ReadAllText();
            MarkdownDocument markdownDocument = Markdig.Markdown.Parse(text, pipeline);
            YDocument frontMatter = markdownDocument.FrontMatterDocument();

            PackageEntryViewModel model = new()
            {
                Title = key,
                Version = packages.GetVersion(),
                Description = frontMatter.GetString("Description"),
                Content = markdownDocument.ToHtml(pipeline)
            };

            return View("Entry", model);
        }
    }
}
