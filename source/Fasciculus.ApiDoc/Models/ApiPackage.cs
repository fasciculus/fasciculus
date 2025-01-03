namespace Fasciculus.ApiDoc.Models
{
    public class ApiPackage : ApiElement
    {
        public string Description { get; set; } = string.Empty;
        public ApiNamespaces Namespaces { get; } = new();
    }
}
