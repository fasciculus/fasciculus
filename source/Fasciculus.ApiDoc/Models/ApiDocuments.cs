namespace Fasciculus.ApiDoc.Models
{
    public class ApiDocuments
    {
        public required ApiPackages Packages { get; init; }
        public required ApiPackage Combined { get; init; }
    }
}
