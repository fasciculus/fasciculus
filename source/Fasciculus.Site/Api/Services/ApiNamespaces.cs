using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Docs.Content.Services;
using Fasciculus.IO;
using Fasciculus.Markdown.Yaml;
using Fasciculus.Site.Api.Models;
using Fasciculus.Yaml;
using Markdig;
using Markdig.Syntax;
using System.IO;

namespace Fasciculus.Site.Api.Services
{
    public class ApiNamespaces
    {
        private readonly NamespaceFiles files;
        private readonly MarkdownPipeline pipeline;

        public ApiNamespaces(NamespaceFiles files, MarkdownPipeline pipeline)
        {
            this.files = files;
            this.pipeline = pipeline;
        }

        public ApiNamespace GetNamespace(INamespaceSymbol symbol)
        {
            string name = symbol.Name;
            string description = string.Empty;
            string content = string.Empty;

            if (files.Contains(name))
            {
                FileInfo file = files.GetFile(name);
                string markdown = file.ReadAllText();
                MarkdownDocument document = Markdig.Markdown.Parse(markdown, pipeline);
                YDocument frontMatter = document.FrontMatterDocument();

                description = frontMatter.GetString("Description");
                content = document.ToHtml(pipeline);
            }

            return new()
            {
                Symbol = symbol,
                Description = description,
                Content = content,
            };
        }
    }
}
