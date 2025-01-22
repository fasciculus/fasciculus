using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.Site.Api.Services;
using Fasciculus.Site.Blog.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Generating.Services
{
    public class GeneratorDocuments : List<string>
    {
        public GeneratorDocuments(ApiContent apiContent, ApiNavigation apiNavigation, BlogContent blogContent)
        {
            AddGlobals();
            AddStatics();
            AddApi(apiContent, apiNavigation);
            AddBlog(blogContent);
        }

        private void AddGlobals()
        {
            Add("/");
            Add("/about.html");
            Add("/privacy.html");
            Add("/releases.html");
        }

        private void AddStatics()
        {
            DirectoryInfo wwwroot = new(Path.GetFullPath("wwwroot"));
            FileInfo[] files = [.. wwwroot.EnumerateFiles("*", SearchOption.AllDirectories)];
            string[] relative = [.. files.Select(file => Path.GetRelativePath(wwwroot.FullName, file.FullName))];
            string[] paths = [.. relative.Select(path => "/" + path.Replace("\\", "/"))];

            paths.Apply(Add);
        }

        private void AddApi(ApiContent apiContent, ApiNavigation apiNavigation)
        {
            Add("/api/");

            foreach (Symbol symbol in apiContent.Index.Symbols)
            {
                if (IsLeaf(symbol.Kind))
                {
                    Add($"/api/{symbol.Link}.html");
                }
                else
                {
                    Add($"/api/{symbol.Link}/");
                }
            }
        }

        private bool IsLeaf(SymbolKind kind)
        {
            return kind switch
            {
                SymbolKind.Field => true,
                SymbolKind.EnumMember => true,
                SymbolKind.Property => true,
                _ => false,
            };
        }

        private void AddBlog(BlogContent blogContent)
        {
            Add("/blog/");

            blogContent.Years.Apply(y => { Add($"/{y.Link}/"); });
            blogContent.Months.Apply(m => { Add($"/{m.Link}/"); });
            blogContent.Entries.Apply(e => { Add($"/{e.Link}.html"); });
        }
    }
}
