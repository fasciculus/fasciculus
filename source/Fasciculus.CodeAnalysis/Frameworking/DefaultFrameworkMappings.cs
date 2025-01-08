using NuGet.Frameworks;
using System;
using static Fasciculus.CodeAnalysis.Frameworking.TargetFrameworkConstants;
using static NuGet.Frameworks.FrameworkConstants;

namespace Fasciculus.CodeAnalysis.Frameworking
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

        /// <summary>
        /// Returns the short version of the given <paramref name="framework"/>.
        /// </summary>
        public string GetProductVersion(NuGetFramework framework)
        {
            Version version = framework.Version;

            return framework.Framework switch
            {
                FrameworkIdentifiers.NetCoreApp => version.ToString(1),
                FrameworkIdentifiers.NetStandard => version.ToString(2),
                _ => version.ToString(),
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
