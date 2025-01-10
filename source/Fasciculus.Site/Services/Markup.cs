using Fasciculus.IO;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Renderers;
using Markdig.Syntax;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Services
{
    public class Markup
    {
        private readonly Yaml yaml;
        private readonly MarkdownPipeline pipeline;

        public Markup(Yaml yaml)
        {
            this.yaml = yaml;

            pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .Build();
        }

        public MarkdownDocument Parse(string markdown)
        {
            return Markdown.Parse(markdown, pipeline);
        }

        public MarkdownDocument Parse(FileInfo file)
            => Parse(file.ReadAllText());

        public T FrontMatter<T>(MarkdownDocument document)
            where T : new()
        {
            YamlFrontMatterBlock? block = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

            if (block is not null)
            {
                string text = string.Join("\r\n", block.Lines);

                return yaml.Deserialize<T>(text);
            }

            return new T();
        }

        public string Render(MarkdownDocument document)
        {
            using StringWriter writer = new();
            HtmlRenderer renderer = new(writer);

            pipeline.Setup(renderer);
            renderer.Render(document);
            writer.Flush();

            return writer.ToString();
        }
    }
}
