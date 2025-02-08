using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Fasciculus.Markdown.ColorCode
{
    /// <summary>
    /// Replaces an existing <see cref="CodeBlockRenderer"/> with a <see cref="ColorCodeRenderer"/>.
    /// </summary>
    public class ColorCodeExtension : IMarkdownExtension
    {
        /// <summary>
        /// Initializes this extension for the specified <paramref name="pipeline"/> builder.
        /// </summary>
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        /// <summary>
        /// Initializes this extension for the specified <paramref name="renderer"/>.
        /// </summary>
        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is not TextRendererBase<HtmlRenderer> htmlRenderer)
            {
                return;
            }

            CodeBlockRenderer? fallbackRenderer = htmlRenderer.ObjectRenderers.FindExact<CodeBlockRenderer>();

            if (fallbackRenderer is null)
            {
                fallbackRenderer = new CodeBlockRenderer();
            }
            else
            {
                htmlRenderer.ObjectRenderers.Remove(fallbackRenderer);
            }

            ColorCodeRenderer colorCodeRenderer = new(fallbackRenderer);

            htmlRenderer.ObjectRenderers.AddIfNotAlready(colorCodeRenderer);
        }
    }
}
