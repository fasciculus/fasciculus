using Fasciculus.IO;
using Fasciculus.Markdown;
using Fasciculus.Markdown.ColorCode;
using Fasciculus.Markdown.FrontMatter;
using Fasciculus.Site.Rendering.Models;
using Markdig;
using Markdig.Syntax;
using System.IO;
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
                .UseFrontMatter()
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
            return deserializer.Deserialize<T>(document.GetFrontMatter());
        }

        public string Render(MarkdownDocument document)
        {
            return document.ToHtml(pipeline);
        }
    }
}
