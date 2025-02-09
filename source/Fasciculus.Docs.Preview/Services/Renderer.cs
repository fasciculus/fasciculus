using Fasciculus.Markdown.ColorCode;
using Fasciculus.Markdown.FrontMatter;
using Fasciculus.Markdown.Svg;
using Markdig;

namespace Fasciculus.Docs.Preview.Services
{
    public class Renderer
    {
        private readonly MarkdownPipeline pipeline;

        public Renderer(GraphicsClient graphics)
        {
            pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .UseFrontMatter()
                .UseAlertBlocks()
                .UseMathematics()
                .UsePipeTables()
                .UseBootstrap()
                .UseSvg(graphics)
                .UseColorCode()
                .Build();
        }

        public string Render(string text)
        {
            return Markdig.Markdown.ToHtml(text, pipeline);
        }
    }
}
