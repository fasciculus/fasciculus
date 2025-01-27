using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.Site.Api.Models
{
    public enum ApiAppliesToLevel
    {
        H2,
        H3,
        H4,
        H5
    }

    public class ApiAppliesTo
    {
        public const string DefaultId = "applies-to";
        public const ApiAppliesToLevel DefaultLevel = ApiAppliesToLevel.H2;

        public TargetProducts Products { get; }

        public ApiAppliesToLevel Level { get; }

        public string Id { get; }

        public ApiAppliesTo(ITargetFrameworks frameworks, ApiAppliesToLevel level, string id)
        {
            Products = frameworks.Products;
            Level = level;
            Id = id;
        }

        public ApiAppliesTo(ITargetFrameworks frameworks, ApiAppliesToLevel level)
            : this(frameworks, level, DefaultId) { }

        public ApiAppliesTo(ITargetFrameworks frameworks, string id)
            : this(frameworks, DefaultLevel, id) { }

        public ApiAppliesTo(ITargetFrameworks frameworks)
            : this(frameworks, DefaultLevel, DefaultId) { }
    }
}
