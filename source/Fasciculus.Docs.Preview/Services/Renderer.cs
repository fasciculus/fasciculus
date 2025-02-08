using Fasciculus.Markdown;
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
                .UseAlertBlocks()
                .UseMathematics()
                .UsePipeTables()
                .UseBootstrap()
                .UseSvg(graphics)
                .Build();
        }

        public string Render(string text)
        {
            return Markdig.Markdown.ToHtml(text, pipeline);
        }
    }
}
