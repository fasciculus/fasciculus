using Fasciculus.ApiDoc.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.ApiDoc
{
    public class ApiMerger
    {
        public ApiPackages MergePackages(IEnumerable<ApiPackage> packages)
            => new(packages.GroupBy(p => p.Name).Select(MergePackageGroup));

        private ApiPackage MergePackageGroup(IGrouping<string, ApiPackage> packageGroup)
        {
            ApiPackage result = new()
            {
                Name = packageGroup.Key
            };

            packageGroup.Apply(p => { result.TargetFrameworks.Add(p.TargetFrameworks); });

            return result;
        }

        public static ApiPackages Merge(IEnumerable<ApiPackage> packages)
        {
            ApiMerger merger = new();

            return merger.MergePackages(packages);
        }
    }
}
