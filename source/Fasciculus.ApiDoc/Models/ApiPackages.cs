using System.Collections;
using System.Collections.Generic;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiPackages : IEnumerable<ApiPackage>
    {
        private readonly Dictionary<string, ApiPackage> packages = [];

        public ApiPackage this[string name] => GetPackage(name);

        private ApiPackage GetPackage(string name)
        {
            if (packages.TryGetValue(name, out ApiPackage? package))
            {
                return package;
            }

            package = new ApiPackage() { Name = name };
            packages[name] = package;

            return package;
        }

        public IEnumerator<ApiPackage> GetEnumerator()
            => packages.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => packages.Values.GetEnumerator();
    }
}
