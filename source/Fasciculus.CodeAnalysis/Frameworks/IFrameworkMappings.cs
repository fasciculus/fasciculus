using NuGet.Frameworks;

namespace Fasciculus.CodeAnalysis.Frameworks
{
    /// <summary>
    /// Maps frameworks to product names.
    /// </summary>
    public interface IFrameworkMappings
    {
        /// <summary>
        /// Returns the product name of the given <paramref name="framework"/>.
        /// </summary>
        public string GetProduct(NuGetFramework framework);
    }
}
