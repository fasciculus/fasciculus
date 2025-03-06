using Fasciculus.CodeAnalysis;
using Fasciculus.CodeAnalysis.Configuration;
using Fasciculus.CodeAnalysis.Indexing;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.IO;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Api.Services
{
    public class ApiContent
    {
        private readonly ICodeAnalyzerResult result;

        public ISymbolIndex Index => result.Index;
        public IPackageSymbol[] Packages => result.Packages;

        public ApiContent()
        {
            result = CodeAnalyzer.Create()
                .WithProjects(GetProjects())
                .Build().Analyze();
        }

        public ISymbol? GetSymbol(UriPath link)
            => Index.TryGetSymbol(link, out ISymbol? symbol) ? symbol : null;

        private static IEnumerable<CodeAnalyzerProject> GetProjects()
        {
            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents();

            foreach (string packageName in SiteConstants.PackageNames)
            {
                DirectoryInfo directory = DirectorySearch.Search(packageName, searchPath).First();

                CodeAnalyzerProject project = new()
                {
                    File = directory.File(packageName + ".csproj"),
                    Repository = new($"https://github.com/fasciculus/fasciculus/tree/main/source/{packageName}/"),
                };

                yield return project;
            }
        }
    }
}
