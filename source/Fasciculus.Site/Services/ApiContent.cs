using Fasciculus.CodeAnalysis;
using Fasciculus.CodeAnalysis.Indexing;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.IO;
using Fasciculus.IO.Searching;
using Fasciculus.Net.Navigating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Services
{
    public class ApiContent
    {
        private readonly CodeAnalyzerResult result;

        public SymbolIndices Indices => result.Indices;
        public PackageList Packages => result.Packages;

        public ApiContent()
        {
            result = CodeAnalyzer.Create().WithProjectFiles(GetProjectFiles()).Build().Analyze();
        }

        public Symbol? GetSymbol(UriPath link)
            => result.Indices.Symbols.TryGetValue(link, out Symbol? symbol) ? symbol : null;

        private static readonly string[] PackageNames =
        [
            "Fasciculus.Core",
            "Fasciculus.Extensions"
        ];

        private static IEnumerable<FileInfo> GetProjectFiles()
        {
            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents;
            DirectoryInfo? directory(string packageName) => DirectorySearch.Search(packageName, searchPath).FirstOrDefault();

            return PackageNames
                .Select(n => Tuple.Create(directory(n), n))
                .Select(t => t.Item1?.File(t.Item2 + ".csproj"))
                .NotNull();
        }
    }
}
