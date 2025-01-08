using Fasciculus.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Frameworking
{
    /// <summary>
    /// Collection of target products
    /// </summary>
    public class TargetProducts : IEnumerable<TargetProduct>
    {
        private readonly Dictionary<string, TargetProduct> products = [];

        /// <summary>
        /// Initializes this collection with the given <paramref name="frameworks"/>.
        /// </summary>
        public TargetProducts(IEnumerable<TargetFramework> frameworks)
        {
            Add(frameworks);
        }

        /// <summary>
        /// Initializes this collection.
        /// </summary>
        public TargetProducts()
            : this([]) { }

        /// <summary>
        /// Adds the given <paramref name="framework"/> to this collection.
        /// </summary>
        public void Add(TargetFramework framework)
            => GetProduct(framework.Product).Add(framework.ProductVersion);

        /// <summary>
        /// Adds the given <paramref name="frameworks"/> to this collection.
        /// </summary>
        public void Add(IEnumerable<TargetFramework> frameworks)
            => frameworks.Apply(Add);

        private TargetProduct GetProduct(string name)
        {
            if (products.TryGetValue(name, out TargetProduct? product))
            {
                return product;
            }

            product = new(name);
            products.Add(name, product);

            return product;
        }

        /// <summary>
        /// Returns an enumerator that iterates through this collection.
        /// </summary>
        public IEnumerator<TargetProduct> GetEnumerator()
            => products.Values.OrderBy(x => x.Name).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => products.Values.OrderBy(x => x.Name).GetEnumerator();
    }
}
