using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.IO;
using Fasciculus.Site.Api.Services;
using Fasciculus.Site.Blog.Services;
using Fasciculus.Site.Specifications.Models;
using Fasciculus.Site.Specifications.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Generating.Services
{
    public class GeneratorDocuments : SortedSet<string>
    {
        public GeneratorDocuments(ApiContent apiContent, BlogContent blogContent, SpecificationContent specificationContent)
        {
            AddGlobals();
            AddStatics();
            AddApi(apiContent);
            AddBlog(blogContent);
            AddConcepts();
            AddSpecifications(specificationContent);
            AddReleases();
            AddLicenses();
        }

        private void AddGlobals()
        {
            Add("/");
            Add("/about.html");
            Add("/privacy.html");
        }

        private void AddStatics()
        {
            //AddStatics("Fasciculus.Site", "wwwroot");
            AddStatics("Fasciculus.Site.Resources", "Resources");
            AddStatics("Fasciculus.Web.Resources", "Bootstrap");
            AddStatics("Fasciculus.Web.Resources", "Katex");
        }

        private void AddStatics(string projectName, string directoryName)
        {
            DirectoryInfo projectDirectory = DirectorySearch.Search(projectName, SearchPath.WorkingDirectoryAndParents()).First();
            DirectoryInfo staticsDirectory = projectDirectory.Combine(directoryName);
            FileInfo[] files = [.. staticsDirectory.EnumerateFiles("*", SearchOption.AllDirectories)];
            string[] relative = [.. files.Select(file => Path.GetRelativePath(staticsDirectory.FullName, file.FullName))];
            string[] paths = [.. relative.Select(path => "/" + path.Replace("\\", "/"))];

            paths.Apply(p => { Add(p); });
        }

        private void AddApi(ApiContent apiContent)
        {
            Add("/api/");

            foreach (ISymbol symbol in apiContent.Index.Symbols)
            {
                SymbolKind kind = symbol.Kind;

                if (ApiIsIncluded(kind))
                {
                    if (ApiIsLeaf(kind))
                    {
                        Add($"/api/{symbol.Link}.html");
                    }
                    else
                    {
                        Add($"/api/{symbol.Link}/");
                    }

                    if (symbol is IEnumSymbol @enum)
                    {
                        if (@enum.Members.Any())
                        {
                            Add($"/api/{@enum.Link}/-Members.html");
                        }
                    }

                    if (symbol is IClassOrInterfaceSymbol cori)
                    {
                        if (cori.Events.Any())
                        {
                            Add($"/api/{cori.Link}/-Events.html");
                        }
                        if (cori.Properties.Any())
                        {
                            Add($"/api/{cori.Link}/-Properties.html");
                        }
                    }

                    if (symbol is IClassSymbol @class)
                    {
                        if (@class.Fields.Any())
                        {
                            Add($"/api/{@class.Link}/-Fields.html");
                        }

                        if (@class.Constructors.Any())
                        {
                            Add($"/api/{@class.Link}/-Constructors.html");
                        }
                    }
                }
            }
        }

        private static bool ApiIsIncluded(SymbolKind kind)
        {
            return kind switch
            {
                SymbolKind.Member => false,
                SymbolKind.Field => false,
                SymbolKind.Event => false,
                SymbolKind.Property => false,
                SymbolKind.Constructor => false,
                SymbolKind.Method => false,
                _ => true,
            };
        }

        private static bool ApiIsLeaf(SymbolKind kind)
        {
            return kind switch
            {
                SymbolKind.Field => true,
                SymbolKind.Member => true,
                SymbolKind.Event => true,
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

        private void AddConcepts()
        {
            Add("/concepts/");
        }

        private void AddSpecifications(SpecificationContent specificationContent)
        {
            Add("/specifications/");

            foreach (SpecificationPackage package in specificationContent.GetPackages())
            {
                Add($"/specifications/{package.Name}/");

                foreach (SpecificationEntry entry in package.GetEntries())
                {
                    Add($"/specifications/{package.Name}/{entry.Id}.html");
                }
            }
        }

        private void AddReleases()
        {
            Add("/releases/");
            Add("/releases/Roadmap.html");
        }

        private void AddLicenses()
        {
            Add("/licenses/");
            SiteConstants.PackageNames.Apply(p => { Add($"/licenses/{p}.html"); });
        }
    }
}
