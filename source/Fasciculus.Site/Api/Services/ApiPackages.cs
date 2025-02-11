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
    public class ApiPackages
    {
        private readonly PackageFiles files;
        private readonly MarkdownPipeline pipeline;

        public ApiPackages(PackageFiles files, MarkdownPipeline pipeline)
        {
            this.files = files;
            this.pipeline = pipeline;
        }

        public ApiPackage GetPackage(IPackageSymbol package)
        {
            string name = package.Name;
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
                Symbol = package,
                Description = description,
                Content = content,
            };
        }
    }
}
