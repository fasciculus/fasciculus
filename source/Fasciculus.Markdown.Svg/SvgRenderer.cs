using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Fasciculus.Markdown.Svg
{
    public class SvgRenderer : HtmlObjectRenderer<SvgBlock>
    {
        protected override void Write(HtmlRenderer renderer, SvgBlock obj)
        {
            renderer.Write(obj.Svg.ToString()).WriteLine();
        }
    }
}
