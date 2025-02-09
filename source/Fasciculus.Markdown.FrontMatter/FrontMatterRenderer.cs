using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Fasciculus.Markdown.FrontMatter
{
    public class FrontMatterRenderer : HtmlObjectRenderer<FrontMatterBlock>
    {
        private readonly IFrontMapperMappings mappings;

        public FrontMatterRenderer(IFrontMapperMappings mappings)
        {
            this.mappings = mappings;
        }

        protected override void Write(HtmlRenderer renderer, FrontMatterBlock block)
        {
            renderer.Write("<table class=\"frontmatter\">").WriteLine();
            renderer.PushIndent("  ");

            foreach (string key in block.Keys)
            {
                if (block.Entries.TryGetValue(key, out string? value))
                {
                    string label = mappings.GetLabel(key) ?? key;
                    string content = mappings.GetContent(key, value) ?? string.Empty;

                    renderer.Write("<tr>").WriteLine();
                    renderer.PushIndent("  ");

                    renderer.Write("<td class=\"frontmatter-label\">");
                    renderer.Write(label);
                    renderer.Write("</td>").WriteLine();

                    renderer.Write("<td class=\"frontmatter-content\">");
                    renderer.Write(content);
                    renderer.Write("</td>").WriteLine();

                    renderer.PopIndent();
                    renderer.Write("</tr>").WriteLine();
                }
            }

            renderer.PopIndent();
            renderer.Write("</table>").WriteLine();
        }
    }
}
