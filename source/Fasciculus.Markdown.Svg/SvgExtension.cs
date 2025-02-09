using Markdig;
using Markdig.Renderers;

namespace Fasciculus.Markdown.Svg
{
    public class SvgExtension : IMarkdownExtension
    {
        private readonly ISvgMappings mappings;

        public SvgExtension(ISvgMappings mappings)
        {
            this.mappings = mappings;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.AddIfNotAlready(new SvgParser(mappings));
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready(new SvgRenderer());
            }
        }
    }
}
