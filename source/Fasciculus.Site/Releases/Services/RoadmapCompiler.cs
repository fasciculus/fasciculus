using Fasciculus.Docs.Content.Services;
using Fasciculus.IO;
using Fasciculus.Site.Releases.Models;
using Markdig;
using System.IO;

namespace Fasciculus.Site.Releases.Services
{
    public class RoadmapCompiler
    {
        private readonly RoadmapFiles files;
        private readonly MarkdownPipeline pipeline;

        public RoadmapCompiler(RoadmapFiles files, MarkdownPipeline pipeline)
        {
            this.files = files;
            this.pipeline = pipeline;
        }

        public Roadmap Compile(string key)
        {
            FileInfo file = files.GetFile(key);
            string html = Markdig.Markdown.ToHtml(file.ReadAllText(), pipeline);

            return new Roadmap
            {
                Html = html
            };
        }
    }
}
