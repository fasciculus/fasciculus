using Fasciculus.Yaml;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using System.Linq;
using YamlDotNet.Serialization;

namespace Fasciculus.Markdown.Yaml
{
    public static class MarkdownDocumentExtensions
    {
        public static string FrontMatter(this MarkdownDocument document)
            => string.Join("\r\n", document.Descendants<YamlFrontMatterBlock>().Select(b => b.Lines.ToString()));

        public static T FrontMatter<T>(this MarkdownDocument document, IDeserializer deserializer)
            => deserializer.Deserialize<T>(document.FrontMatter());

        public static T FrontMatter<T>(this MarkdownDocument document)
            => document.FrontMatter<T>(YDeserializer.Instance);
    }
}
