using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiPackages : IEnumerable<ApiPackage>
    {
        private readonly Dictionary<string, ApiPackage> packages = [];

        public bool TryGetPackage(string name, [NotNullWhen(true)] out ApiPackage? package)
            => packages.TryGetValue(name, out package);

        //public ApiPackage this[string name] => packages[name];

        internal ApiPackages(IEnumerable<ApiPackage> packages)
        {
            this.packages = packages.ToDictionary(p => p.Name);
        }

        public IEnumerator<ApiPackage> GetEnumerator()
            => packages.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => packages.Values.GetEnumerator();
    }
}
