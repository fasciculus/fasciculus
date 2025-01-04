using Fasciculus.CodeAnalysis.Models;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiPackages : IEnumerable<ApiPackage>
    {
        private readonly Dictionary<string, ApiPackage> packages;

        public bool TryGetPackage(string name, [NotNullWhen(true)] out ApiPackage? package)
            => packages.TryGetValue(name, out package);

        public ApiPackages(PackageCollection packages)
        {
            this.packages = packages.Select(p => new ApiPackage(p)).ToDictionary(p => p.Name);
        }

        public IEnumerator<ApiPackage> GetEnumerator()
            => packages.Values.OrderBy(p => p.Name).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => packages.Values.OrderBy(p => p.Name).GetEnumerator();
    }
}
