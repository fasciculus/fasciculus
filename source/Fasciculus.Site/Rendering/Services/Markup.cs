using Fasciculus.IO;
using Markdig;
using Markdig.Syntax;
using System.IO;

namespace Fasciculus.Site.Rendering.Services
{
    public class Markup
    {
        private readonly MarkdownPipeline pipeline;

        public Markup(MarkdownPipeline pipeline)
        {
            this.pipeline = pipeline;
        }

        public MarkdownDocument Parse(string markdown)
        {
            return Markdig.Markdown.Parse(markdown, pipeline);
        }

        public MarkdownDocument Parse(FileInfo file)
            => Parse(file.ReadAllText());

        public string Render(MarkdownDocument document)
        {
            return document.ToHtml(pipeline);
        }
    }
}
