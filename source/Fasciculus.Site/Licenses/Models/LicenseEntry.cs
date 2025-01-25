using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using System;

namespace Fasciculus.Site.Licenses.Models
{
    public class LicenseEntry : IEquatable<LicenseEntry>, IComparable<LicenseEntry>
    {
        public required PackageIdentity Identity { get; init; }

        public required string License { get; init; }

        public required Uri? LicenseUrl { get; init; }

        public static LicenseEntry Create(IPackageSearchMetadata metadata)
        {
            LicenseMetadata? licenseMetadata = metadata.LicenseMetadata;
            string license = licenseMetadata is null ? "Proprietary" : licenseMetadata.License;

            return new()
            {
                Identity = metadata.Identity,
                License = license,
                LicenseUrl = metadata.LicenseUrl,
            };
        }

        public override int GetHashCode()
            => PackageIdentityComparer.Default.GetHashCode(Identity);

        public bool Equals(LicenseEntry? other)
            => PackageIdentityComparer.Default.Equals(Identity, other?.Identity);

        public override bool Equals(object? obj)
            => Equals(obj as LicenseEntry);

        public int CompareTo(LicenseEntry? other)
            => PackageIdentityComparer.Default.Compare(Identity, other?.Identity);
    }
}
