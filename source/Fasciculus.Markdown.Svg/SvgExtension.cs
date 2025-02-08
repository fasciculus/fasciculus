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
            if (!pipeline.BlockParsers.Contains<SvgParser>())
            {
                pipeline.BlockParsers.Add(new SvgParser(mappings));
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<SvgRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Add(new SvgRenderer());
                }
            }
        }
    }
}
