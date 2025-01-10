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

        public Markup(Yaml yaml)
        {
            this.yaml = yaml;
        }

        public MarkdownDocument Parse(string markdown)
        {
            MarkdownPipeline pipeline = CreatePipeline();

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

            _ = CreatePipeline(renderer);

            renderer.Render(document);
            writer.Flush();

            return writer.ToString();
        }

        private static MarkdownPipeline CreatePipeline(IMarkdownRenderer? renderer = null)
        {
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .Build();

            if (renderer is not null)
            {
                pipeline.Setup(renderer);
            }

            return pipeline;
        }
    }
}
