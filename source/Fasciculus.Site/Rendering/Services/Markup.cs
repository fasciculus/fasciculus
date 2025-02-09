using Fasciculus.IO;
using Fasciculus.Markdown.ColorCode;
using Fasciculus.Site.Rendering.Models;
using Fasciculus.Site.Rendering.Rendering;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;

namespace Fasciculus.Site.Rendering.Services
{
    public class Markup
    {
        private readonly IDeserializer deserializer;
        private readonly MarkdownPipeline pipeline;

        public Markup(IDeserializer deserializer)
        {
            this.deserializer = deserializer;

            pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .UseAlertBlocks()
                .UseColorCode()
                .UseMathematics()
                .UsePipeTables()
                .UseBootstrap()
                .Build();
        }

        public MarkdownDocument Parse(string markdown)
        {
            return Markdig.Markdown.Parse(markdown, pipeline);
        }

        public MarkdownDocument Parse(FileInfo file)
            => Parse(file.ReadAllText());

        public T FrontMatter<T>(MarkdownDocument document)
            where T : IFrontMatter, new()
        {
            YamlFrontMatterBlock? block = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

            if (block is not null)
            {
                string text = string.Join("\r\n", block.Lines);

                return deserializer.Deserialize<T>(text);
            }

            return new T();
        }

        public string Render(MarkdownDocument document, IFrontMatter? frontMatter = null)
        {
            using StringWriter writer = new();
            IEnumerable<FrontMatterEntry> entries = frontMatter?.GetEntries() ?? [];
            MarkupRenderer renderer = new(writer, pipeline, entries);

            renderer.Render(document);
            writer.Flush();

            return writer.ToString();
        }
    }
}
