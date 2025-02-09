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
        private readonly MarkdownPipeline pipeline;

        public SpecificationCompiler(MarkdownPipeline pipeline)
        {
            this.pipeline = pipeline;
        }

        public SpecificationEntry Compile(FileInfo file)
        {
            MarkdownDocument document = Markdig.Markdown.Parse(file.ReadAllText(), pipeline);
            SpecificationFrontMatter frontMatter = document.FrontMatter<SpecificationFrontMatter>();
            string id = file.Name[..file.Name.IndexOf('.')];
            string html = document.ToHtml(pipeline);

            return new()
            {
                Id = id,
                Title = frontMatter.Title,
                Html = html
            };
        }
    }
}
