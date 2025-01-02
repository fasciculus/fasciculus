using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.GitHub.Services
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

            apiProvider.Packages.Apply(p => { Add($"/api/pkg/{p.Name}.html"); });
        }
    }
}
