using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Renderers;
using Markdig.Syntax;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace Fasciculus.Markdown.FrontMatter
{
    public class FrontMatterExtension : IMarkdownExtension
    {
        private readonly IDeserializer deserializer;
        private readonly IFrontMapperMappings mappings;

        public FrontMatterExtension(IDeserializer deserializer, IFrontMapperMappings mappings)
        {
            this.deserializer = deserializer;
            this.mappings = mappings;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.BlockParsers.Contains<FrontMatterParser>())
            {
                pipeline.BlockParsers.Add(new FrontMatterParser());
            }

            pipeline.DocumentProcessed -= OnDocumentProcessed;
            pipeline.DocumentProcessed += OnDocumentProcessed;
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<FrontMatterRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Add(new FrontMatterRenderer(mappings));
                }
            }
        }

        private void OnDocumentProcessed(MarkdownDocument document)
        {
            Dictionary<string, string> entries = DeserializeFrontMatters(document);

            foreach (FrontMatterBlock block in document.Descendants<FrontMatterBlock>())
            {
                block.Entries = entries;
            }
        }

        private Dictionary<string, string> DeserializeFrontMatters(MarkdownDocument document)
        {
            IEnumerable<KeyValuePair<string, string>> entries = [];

            foreach (YamlFrontMatterBlock block in document.Descendants<YamlFrontMatterBlock>())
            {
                string yaml = string.Join("\r\n", block.Lines);
                Dictionary<string, string> dictionary = deserializer.Deserialize<Dictionary<string, string>>(yaml);

                entries = entries.Concat(dictionary);
            }

            return entries.ToDictionary(e => e.Key, e => e.Value);
        }
    }
}
