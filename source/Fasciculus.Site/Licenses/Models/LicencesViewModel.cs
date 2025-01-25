using Fasciculus.Site.Models;

namespace Fasciculus.Site.Licenses.Models
{
    public class LicencesViewModel : ViewModel
    {
        public required LicenseList[] LicenseLists { get; init; }
    }
}
