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
    public class NamespaceController : Controller
    {
        private readonly NamespaceClient namespaces;
        private readonly MarkdownPipeline pipeline;

        public NamespaceController(NamespaceClient namespaces, MarkdownPipeline pipeline)
        {
            this.namespaces = namespaces;
            this.pipeline = pipeline;
        }

        [Route("/Namespaces/")]
        public IActionResult Index()
        {
            NamespaceIndexViewModel model = new()
            {
                Title = "Namespaces",
                Version = namespaces.GetVersion(),
                Keys = namespaces.GetKeys(),
            };

            return View("Index", model);
        }

        [Route("/Namespaces/{key}")]
        public IActionResult Entry(string key)
        {
            string text = namespaces.GetFile(key).ReadAllText();
            MarkdownDocument markdownDocument = Markdig.Markdown.Parse(text, pipeline);
            YDocument frontMatter = markdownDocument.FrontMatterDocument();

            NamespaceEntryViewModel model = new()
            {
                Title = key,
                Version = namespaces.GetVersion(),
                Description = frontMatter.GetString("Description"),
                Content = markdownDocument.ToHtml(pipeline)
            };

            return View("Entry", model);
        }
    }
}
