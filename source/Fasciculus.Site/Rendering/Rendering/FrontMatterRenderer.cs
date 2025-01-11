using Fasciculus.Collections;
using Fasciculus.Site.Rendering.Models;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using System.Collections.Generic;

namespace Fasciculus.Site.Rendering.Rendering
{
    public class FrontMatterRenderer : HtmlObjectRenderer<HeadingBlock>
    {
        private readonly List<IMarkdownObjectRenderer> renderers;
        private readonly List<FrontMatterEntry> entries;

        public FrontMatterRenderer(IEnumerable<IMarkdownObjectRenderer> renderers, IEnumerable<FrontMatterEntry> entries)
        {
            this.renderers = [.. renderers];
            this.entries = [.. entries];
        }

        protected override void Write(HtmlRenderer renderer, HeadingBlock heading)
        {
            renderers.Apply(r => { r.Write(renderer, heading); });

            if (entries.Count > 0 && heading.Level == 1 && renderer.EnableHtmlForBlock)
            {
                renderer.Write("<table class=\"fsc-frontmatter\">").WriteLine();
                renderer.PushIndent("  ");

                renderer.Write("<tbody>").WriteLine();
                renderer.PushIndent("  ");

                foreach (FrontMatterEntry entry in entries)
                {
                    renderer.Write("<tr>").WriteLine();
                    renderer.PushIndent("  ");

                    renderer.Write("<td>").Write(entry.Label).Write(":</td>").WriteLine();
                    renderer.Write("<td>").Write(entry.Value).Write("</td>").WriteLine();

                    renderer.PopIndent();
                    renderer.Write("</tr>").WriteLine();
                }

                renderer.PopIndent();
                renderer.Write("</tbody>").WriteLine();

                renderer.PopIndent();
                renderer.Write("</table>").WriteLine();
            }
        }
    }
}
