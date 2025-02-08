using Markdig;

namespace Fasciculus.Markdown.ColorCode
{
    /// <summary>
    /// Helper class to add the <see cref="ColorCodeExtension"/>
    /// </summary>
    public static class ColorCodeConfiguration
    {
        /// <summary>
        /// Adds the <see cref="ColorCodeExtension"/> to the pipeline.
        /// </summary>
        public static MarkdownPipelineBuilder UseColorCode(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<ColorCodeExtension>();

            return pipeline;
        }
    }
}
