using NuGet.Frameworks;
using System;
using System.Diagnostics;

namespace Fasciculus.CodeAnalysis.Frameworks
{
    /// <summary>
    /// A wrapper around a <see cref="NuGetFramework"/> with additional properties.
    /// </summary>
    [DebuggerDisplay("{NuGetFramework}")]
    public class TargetFramework : IEquatable<TargetFramework>
    {
        /// <summary>
        /// Unsupported framework.
        /// </summary>
        public static readonly TargetFramework UnsupportedFramework = new(NuGetFramework.UnsupportedFramework, "", "");

        /// <summary>
        /// The wrapped <see cref="NuGetFramework"/>.
        /// </summary>
        public NuGetFramework NuGetFramework { get; }

        /// <summary>
        /// The product of the wrapped framework.
        /// </summary>
        public string Product { get; }

        /// <summary>
        /// The short version of the wrapped framework.
        /// </summary>
        public string ProductVersion { get; }

        private TargetFramework(NuGetFramework framework, string product, string productVersion)
        {
            NuGetFramework = framework;
            Product = product;
            ProductVersion = productVersion;
        }

        /// <summary>
        /// Creates a TargetFramework from the given moniker using the given mappings.
        /// </summary>
        public static TargetFramework Parse(string moniker, IFrameworkNameProvider frameworkNameProvider, IFrameworkMappings productMappings)
        {
            NuGetFramework framework = NuGetFramework.Parse(moniker, frameworkNameProvider);
            string product = productMappings.GetProduct(framework);
            string productVersion = productMappings.GetProductVersion(framework);

            return new(framework, product, productVersion);
        }

        /// <summary>
        /// Creates a TargetFramework from the given moniker using default mappings.
        /// </summary>
        public static TargetFramework Parse(string moniker)
            => Parse(moniker, DefaultFrameworkNameProvider.Instance, DefaultFrameworkMappings.Instance);

        /// <summary>
        /// Returns whether this target framework is equal to the <paramref name="other"/> target framework.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Equals(TargetFramework? other)
            => other is not null && NuGetFramework.Equals(other.NuGetFramework);

        /// <summary>
        /// Returns whether this target framework is equal to the <paramref name="obj"/> target framework.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
            => Equals(obj as TargetFramework);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => NuGetFramework.GetHashCode();
    }
}
