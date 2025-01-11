using Fasciculus.Site.Rendering.Models;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.Site.Rendering.Rendering
{
    public sealed class MarkupRenderer : HtmlRenderer
    {
        public MarkupRenderer(TextWriter writer, MarkdownPipeline pipeline, IEnumerable<FrontMatterEntry> entries)
            : base(writer)
        {
            pipeline.Setup(this);

            IMarkdownObjectRenderer[] renderers = [.. ObjectRenderers.FindAll(r => r is HeadingRenderer)];
            FrontMatterRenderer renderer = new(renderers, entries);

            ObjectRenderers.RemoveAll(r => r is HeadingRenderer);
            ObjectRenderers.Add(renderer);
        }
    }
}
