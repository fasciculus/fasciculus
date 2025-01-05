using Fasciculus.ApiDoc.Models;
using Fasciculus.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Services
{
    public class Documents : List<string>
    {
        public Documents(ApiProvider apiProvider)
        {
            AddGlobals();
            AddStatics();
            AddApi(apiProvider);
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

        private void AddApi(ApiProvider apiProvider)
        {
            Add("/api/");

            foreach (ApiPackage package in apiProvider.Packages.Append(apiProvider.Combined))
            {
                Add($"/api/{package.Link}/");

                foreach (ApiNamespace @namespace in package.Namespaces)
                {
                    Add($"/api/{@namespace.Link}/");

                    foreach (ApiClass @class in @namespace.Classes)
                    {
                        Add($"/api/{@class.Link}/");
                    }
                }
            }
        }
    }
}
