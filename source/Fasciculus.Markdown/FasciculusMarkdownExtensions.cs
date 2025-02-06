using Fasciculus.Markdown.Extensions.Svg;
using Markdig;

namespace Fasciculus.Markdown
{
    public static class FasciculusMarkdownExtensions
    {
        public static MarkdownPipelineBuilder UseSvg(this MarkdownPipelineBuilder pipeline, ISvgMappings mappings)
        {
            pipeline.Extensions.ReplaceOrAdd<SvgExtension>(new SvgExtension(mappings));

            return pipeline;
        }
    }
}
