using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Fasciculus.Experiments.SyntaxHighlighting
{
    public class ColorCodeExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is not TextRendererBase<HtmlRenderer> htmlRenderer)
            {
                return;
            }

            CodeBlockRenderer? codeBlockRenderer = htmlRenderer.ObjectRenderers.FindExact<CodeBlockRenderer>();

            if (codeBlockRenderer is not null)
            {
                htmlRenderer.ObjectRenderers.Remove(codeBlockRenderer);
            }
            else
            {
                codeBlockRenderer = new CodeBlockRenderer();
            }

            ColorCodeRenderer colorCodeBlockRenderer = new(codeBlockRenderer);

            htmlRenderer.ObjectRenderers.AddIfNotAlready(colorCodeBlockRenderer);
        }
    }
}
