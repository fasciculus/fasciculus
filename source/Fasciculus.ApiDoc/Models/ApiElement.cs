namespace Fasciculus.ApiDoc.Models
{
    public class ApiElement
    {
        public required string Name { get; init; }

        public ApiTargetFrameworks TargetFrameworks { get; } = new();
    }
}
