using Fasciculus.Yaml;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using System.Linq;
using YamlDotNet.Serialization;

namespace Fasciculus.Markdown.Yaml
{
    public static class MarkdownDocumentExtensions
    {
        public static string FrontMatterText(this MarkdownDocument document)
            => string.Join("\r\n", document.Descendants<YamlFrontMatterBlock>().Select(b => b.Lines.ToString()));

        public static T FrontMatterObject<T>(this MarkdownDocument document, IDeserializer deserializer)
            => deserializer.Deserialize<T>(document.FrontMatterText());

        public static T FrontMatterObject<T>(this MarkdownDocument document)
            => document.FrontMatterObject<T>(YDeserializer.Instance);

        public static YDocument FrontMatterDocument(this MarkdownDocument document, IDeserializer deserializer)
            => YDocument.Deserialize(FrontMatterText(document), deserializer);

        public static YDocument FrontMatterDocument(this MarkdownDocument document)
            => FrontMatterDocument(document, YDeserializer.Instance);
    }
}
