using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiNamespaces : IEnumerable<ApiNamespace>
    {
        private readonly SortedSet<ApiNamespace> namespaces = [];

        public ApiNamespace Add(string name)
        {
            ApiNamespace? result = namespaces.FirstOrDefault(n => n.Name == name);

            if (result is null)
            {
                result = new() { Name = name };
                namespaces.Add(result);
            }

            return result;
        }

        public void Add(ApiNamespaces other)
        {
            foreach (ApiNamespace otherNamespace in other)
            {
                ApiNamespace? existing = namespaces.FirstOrDefault(ns => ns.Name == otherNamespace.Name);

                if (existing is null)
                {
                    namespaces.Add(otherNamespace);
                }
                else
                {

                }
            }
        }

        public IEnumerator<ApiNamespace> GetEnumerator()
            => namespaces.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => namespaces.GetEnumerator();
    }
}
