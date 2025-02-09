using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using System.Linq;

namespace Fasciculus.Markdown
{
    public static class MarkdownDocumentExtensions
    {
        public static string GetFrontMatter(this MarkdownDocument document)
            => string.Join("\r\n", document.Descendants<YamlFrontMatterBlock>().Select(b => b.Lines.ToString()));
    }
}
