using Fasciculus.Docs.Content.Services;
using Fasciculus.Site.Specifications.Models;
using System.Linq;

namespace Fasciculus.Site.Specifications.Services
{
    public class SpecificationContent
    {
        private readonly SpecificationPackages packages;

        public SpecificationContent(SpecificationFiles files, SpecificationCompiler compiler)
        {
            packages = new(files.GetKeys().Select(compiler.Compile));
        }

        public SpecificationPackage[] GetPackages()
        {
            return [.. packages.GetPackages()];
        }

        public SpecificationPackage GetPackage(string name)
        {
            return packages.GetPackage(name);
        }
    }
}
