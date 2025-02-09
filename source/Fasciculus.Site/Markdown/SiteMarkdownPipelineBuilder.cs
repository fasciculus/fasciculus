using Fasciculus.Markdown.ColorCode;
using Fasciculus.Markdown.FrontMatter;
using Fasciculus.Markdown.Svg;
using Markdig;

namespace Fasciculus.Site.Markdown
{
    public class SiteMarkdownPipelineBuilder : MarkdownPipelineBuilder
    {
        public SiteMarkdownPipelineBuilder(ISvgMappings svgMappings)
        {
            this.UseYamlFrontMatter()
                .UseFrontMatter()
                .UseAlertBlocks()
                .UseColorCode()
                .UseMathematics()
                .UsePipeTables()
                .UseBootstrap()
                .UseSvg(svgMappings);
        }
    }
}
