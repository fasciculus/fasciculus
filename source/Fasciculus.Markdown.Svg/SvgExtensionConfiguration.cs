using Markdig;

namespace Fasciculus.Markdown.Svg
{
    public static class SvgExtensionConfiguration
    {
        public static MarkdownPipelineBuilder UseSvg(this MarkdownPipelineBuilder pipeline, ISvgMappings mappings)
        {
            pipeline.Extensions.ReplaceOrAdd<SvgExtension>(new SvgExtension(mappings));

            return pipeline;
        }
    }
}
