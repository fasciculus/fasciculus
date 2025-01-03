using NuGet.Frameworks;
using System;
using static Fasciculus.CodeAnalysis.Frameworks.TargetFrameworkConstants;
using static NuGet.Frameworks.FrameworkConstants;

namespace Fasciculus.CodeAnalysis.Frameworks
{
    /// <summary>
    /// Default implementation of <see cref="IFrameworkMappings"/>.
    /// </summary>
    public class DefaultFrameworkMappings : IFrameworkMappings
    {
        private DefaultFrameworkMappings() { }

        /// <summary>
        /// Returns the product name of the given <paramref name="framework"/>.
        /// </summary>
        public string GetProduct(NuGetFramework framework)
        {
            return framework.Framework switch
            {
                FrameworkIdentifiers.NetCoreApp => ProductIdentifiers.Net,
                FrameworkIdentifiers.NetStandard => ProductIdentifiers.NetStandard,
                _ => string.Empty,
            };
        }

        private static readonly Lazy<DefaultFrameworkMappings> instance
            = new(() => new DefaultFrameworkMappings(), true);

        /// <summary>
        /// Returns the singleton instance of this class.
        /// </summary>
        public static DefaultFrameworkMappings Instance
            => instance.Value;
    }
}
