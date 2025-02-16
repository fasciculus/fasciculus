using Fasciculus.Docs.Content.Services;
using Fasciculus.IO;
using Fasciculus.Markdown.Yaml;
using Fasciculus.Site.Specifications.Models;
using Markdig;
using Markdig.Syntax;
using System.IO;

namespace Fasciculus.Site.Specifications.Services
{
    public class SpecificationCompiler
    {
        private readonly SpecificationFiles files;
        private readonly MarkdownPipeline pipeline;

        public SpecificationCompiler(SpecificationFiles files, MarkdownPipeline pipeline)
        {
            this.files = files;
            this.pipeline = pipeline;
        }

        public SpecificationEntry Compile(string key)
        {
            FileInfo file = files.GetFile(key);
            MarkdownDocument document = Markdig.Markdown.Parse(file.ReadAllText(), pipeline);
            SpecificationFrontMatter frontMatter = document.FrontMatterObject<SpecificationFrontMatter>();
            string[] parts = key.Split('|');
            string package = parts[0];
            string id = parts[1];
            string html = document.ToHtml(pipeline);

            return new()
            {
                Package = package,
                Id = id,
                Title = frontMatter.Title,
                Html = html
            };
        }
    }
}
