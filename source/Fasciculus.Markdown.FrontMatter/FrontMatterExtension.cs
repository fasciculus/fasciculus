using Fasciculus.Yaml;
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
            IEnumerable<string> yaml = document.Descendants<YamlFrontMatterBlock>().Select(b => b.Lines.ToString());
            YDictionary entries = YDictionary.Deserialize(string.Join("\r\n", yaml), deserializer);

            foreach (FrontMatterBlock block in document.Descendants<FrontMatterBlock>())
            {
                block.Entries = entries;
            }
        }
    }
}
