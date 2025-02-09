using Fasciculus.Yaml;
using Markdig;
using YamlDotNet.Serialization;

namespace Fasciculus.Markdown.FrontMatter
{
    public static class FrontMatterConfiguration
    {
        public static MarkdownPipelineBuilder UseFrontMatter(this MarkdownPipelineBuilder pipeline, IDeserializer deserializer,
            IFrontMapperMappings mappings)
        {
            FrontMatterExtension extension = new(deserializer, mappings);

            pipeline.Extensions.ReplaceOrAdd<FrontMatterExtension>(extension);

            return pipeline;
        }

        public static MarkdownPipelineBuilder UseFrontMatter(this MarkdownPipelineBuilder pipeline, IDeserializer deserializer)
            => pipeline.UseFrontMatter(deserializer, FrontMapperMappings.Empty());

        public static MarkdownPipelineBuilder UseFrontMatter(this MarkdownPipelineBuilder pipeline, IFrontMapperMappings mappings)
            => pipeline.UseFrontMatter(YDeserializer.Instance, mappings);

        public static MarkdownPipelineBuilder UseFrontMatter(this MarkdownPipelineBuilder pipeline)
            => pipeline.UseFrontMatter(FrontMapperMappings.Empty());
    }
}
