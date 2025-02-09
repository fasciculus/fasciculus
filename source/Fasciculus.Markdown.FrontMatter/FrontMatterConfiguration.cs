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
        {
            IFrontMapperMappings mappings = new FrontMapperMappings();

            return pipeline.UseFrontMatter(deserializer, mappings);
        }

        public static MarkdownPipelineBuilder UseFrontMatter(this MarkdownPipelineBuilder pipeline, IFrontMapperMappings mappings)
        {
            DeserializerBuilder builder = new DeserializerBuilder();
            IDeserializer deserializer = builder.Build();

            return pipeline.UseFrontMatter(deserializer, mappings);
        }

        public static MarkdownPipelineBuilder UseFrontMatter(this MarkdownPipelineBuilder pipeline)
        {
            IFrontMapperMappings mappings = new FrontMapperMappings();

            return pipeline.UseFrontMatter(mappings);
        }
    }
}
