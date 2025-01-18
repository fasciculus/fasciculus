using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiViewModel : ViewModel
    {
        public string RepositoryPrefix { get; } = "https://github.com/fasciculus/fasciculus/";
    }
}
