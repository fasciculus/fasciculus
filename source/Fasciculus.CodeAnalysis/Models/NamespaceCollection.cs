using Fasciculus.CodeAnalysis.Frameworks;
using Fasciculus.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    /// <summary>
    /// Collection of namespaces.
    /// </summary>
    public class NamespaceCollection : IElementCollection<NamespaceInfo>
    {
        private readonly Dictionary<string, NamespaceInfo> namespaces = [];

        /// <summary>
        /// Adds the given <paramref name="namespace"/> to this collection, merging it into an existing package of the
        /// same name if such one exists.
        /// </summary>
        public NamespaceInfo Add(NamespaceInfo @namespace)
        {
            if (namespaces.TryGetValue(@namespace.Name, out NamespaceInfo? existing))
            {
                existing.Add(@namespace);

                return existing;
            }
            else
            {
                namespaces[@namespace.Name] = @namespace;

                return @namespace;
            }
        }

        /// <summary>
        /// Adds a new namespace with the given <paramref name="name"/> or returns an existing namespace with that name.
        /// </summary>
        public NamespaceInfo Add(string name)
        {
            if (namespaces.TryGetValue(name, out NamespaceInfo? existing))
            {
                return existing;
            }

            NamespaceInfo @namespace = new()
            {
                Name = name,
            };

            return Add(@namespace);
        }

        /// <summary>
        /// Adds the given <paramref name="namespaces"/> to this collection, merging them with existing packages of the
        /// same name if such exists.
        /// </summary>
        public void Add(IEnumerable<NamespaceInfo> namespaces)
            => namespaces.Apply(n => { Add(n); });

        /// <summary>
        /// Adds the given <paramref name="framework"/>
        /// </summary>
        public void Add(TargetFramework framework)
            => namespaces.Values.Apply(n => { n.Add(framework); });

        /// <summary>
        /// Returns an enumerator that iterates through this collection.
        /// </summary>
        public IEnumerator<NamespaceInfo> GetEnumerator()
            => namespaces.Values.OrderBy(n => n.Name).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => namespaces.Values.OrderBy(n => n.Name).GetEnumerator();
    }
}
