using Microsoft.Extensions.DependencyInjection;

namespace Fasciculus.Docs.Preview.Models
{
    public class DebugServicesViewModel : ViewModel
    {
        public required ServiceDescriptor[] Descriptors { get; init; }
    }
}
