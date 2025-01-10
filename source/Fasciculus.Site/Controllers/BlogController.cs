using Fasciculus.IO;
using Fasciculus.Site.Models;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Renderers;
using Markdig.Syntax;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;

namespace Fasciculus.Site.Controllers
{
    public class BlogController : Controller
    {
        public class BlogFrontMatter
        {
            public string Title { get; set; } = string.Empty;
        }

        [Route("/blog/")]
        public IActionResult Blog()
        {
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .Build();

            using StringWriter writer = new();
            HtmlRenderer renderer = new(writer);

            pipeline.Setup(renderer);

            FileInfo file = SpecialDirectories.WorkingDirectory
                .Combine("Blog", "Documents", "2025", "01").File("010_Test.md");

            string markdown = file.ReadAllText();
            MarkdownDocument document = Markdown.Parse(markdown, pipeline);
            YamlFrontMatterBlock? frontMatterBlock = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
            BlogFrontMatter frontMatter = new();

            if (frontMatterBlock is not null)
            {
                string frontMatterYaml = string.Join("\r\n", frontMatterBlock.Lines);
                IDeserializer deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().WithCaseInsensitivePropertyMatching().Build();
                frontMatter = deserializer.Deserialize<BlogFrontMatter>(frontMatterYaml);
            }

            renderer.Render(document);
            writer.Flush();

            string content = writer.ToString();

            BlogDocument blogDocument = new()
            {
                Title = frontMatter.Title,
                Content = content
            };

            return View(blogDocument);
        }
    }
}
