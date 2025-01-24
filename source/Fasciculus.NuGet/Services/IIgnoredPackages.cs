namespace Fasciculus.NuGet.Services
{
    public interface IIgnoredPackages
    {
        public bool IsIgnored(string packageId);
    }
}
