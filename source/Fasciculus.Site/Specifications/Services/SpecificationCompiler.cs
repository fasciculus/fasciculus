using Fasciculus.Site.Rendering.Services;
using Fasciculus.Site.Specifications.Models;
using Markdig.Syntax;
using System.IO;

namespace Fasciculus.Site.Specifications.Services
{
    public class SpecificationCompiler
    {
        private readonly Markup markup;

        public SpecificationCompiler(Markup markup)
        {
            this.markup = markup;
        }

        public SpecificationEntry Compile(FileInfo file)
        {
            MarkdownDocument markdown = markup.Parse(file);
            string id = file.Name[..file.Name.IndexOf('.')];
            string title = "Fixed Point Algorithms";
            string content = markup.Render(markdown);

            return new()
            {
                Id = id,
                Title = title,
                Content = content
            };
        }
    }
}
