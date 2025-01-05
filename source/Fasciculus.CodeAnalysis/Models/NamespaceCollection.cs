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
        /// Adds a new namespace with the given <paramref name="name"/> or returns an existing namespace with that name.
        /// </summary>
        public NamespaceInfo Add(string name)
        {
            if (namespaces.TryGetValue(name, out NamespaceInfo? existing))
            {
                return existing;
            }

            NamespaceInfo @namespace = new(name);

            return MergeWith(@namespace);
        }

        /// <summary>
        /// Adds the given <paramref name="namespace"/> to this collection, merging it into an existing namespace of the
        /// same name if such one exists.
        /// </summary>
        public NamespaceInfo MergeWith(NamespaceInfo @namespace)
        {
            if (namespaces.TryGetValue(@namespace.Name, out NamespaceInfo? existing))
            {
                existing.MergeWith(@namespace);

                return existing;
            }
            else
            {
                namespaces[@namespace.Name] = @namespace;

                return @namespace;
            }
        }

        /// <summary>
        /// Adds the given <paramref name="namespaces"/> to this collection, merging them with existing packages of the
        /// same name if such exists.
        /// </summary>
        public void MergeWith(IEnumerable<NamespaceInfo> namespaces)
            => namespaces.Apply(n => { MergeWith(n); });

        /// <summary>
        /// Adds the given <paramref name="framework"/>
        /// </summary>
        public void AddFramework(TargetFramework framework)
            => namespaces.Values.Apply(n => { n.AddFramework(framework); });

        /// <summary>
        /// Adds the given <paramref name="name"/> to the packages this namespaces occurs in.
        /// </summary>
        public void AddPackage(string name)
            => namespaces.Values.Apply(n => { n.AddPackage(name); });

        /// <summary>
        /// Returns an enumerator that iterates through this collection.
        /// </summary>
        public IEnumerator<NamespaceInfo> GetEnumerator()
            => namespaces.Values.OrderBy(n => n.Name).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => namespaces.Values.OrderBy(n => n.Name).GetEnumerator();
    }
}
