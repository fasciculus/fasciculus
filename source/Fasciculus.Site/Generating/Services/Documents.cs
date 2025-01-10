using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.Site.Api.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Generating.Services
{
    public class Documents : List<string>
    {
        public Documents(ApiContent apiContent, ApiNavigation apiNavigation)
        {
            AddGlobals();
            AddStatics();
            AddApi(apiContent, apiNavigation);
            AddBlog();
        }

        private void AddGlobals()
        {
            Add("/");
            Add("/about.html");
            Add("/privacy.html");
        }

        private void AddStatics()
        {
            DirectoryInfo wwwroot = new(Path.GetFullPath("wwwroot"));
            FileInfo[] files = [.. wwwroot.EnumerateFiles("*", SearchOption.AllDirectories)];
            string[] relative = [.. files.Select(file => Path.GetRelativePath(wwwroot.FullName, file.FullName))];
            string[] paths = [.. relative.Select(path => "/" + path.Replace("\\", "/"))];

            paths.Apply(Add);
        }

        private static readonly SortedSet<SymbolKind> DocumentsSymbolKinds =
            [
                SymbolKind.Package,
                SymbolKind.Namespace,
                SymbolKind.Class,
            ];

        private void AddApi(ApiContent apiContent, ApiNavigation apiNavigation)
        {
            Add("/api/");

            foreach (Symbol symbol in apiContent.Indices.Symbols.Values)
            {
                if (DocumentsSymbolKinds.Contains(symbol.Kind))
                {
                    Add($"/api/{symbol.Link}/");
                }
            }
        }

        private void AddBlog()
        {
            Add("/blog/");
        }
    }
}
