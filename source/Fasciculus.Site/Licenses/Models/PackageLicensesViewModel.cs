using Fasciculus.Site.Models;

namespace Fasciculus.Site.Licenses.Models
{
    public class PackageLicensesViewModel : ViewModel
    {
        public required LicenseList Licenses { get; init; }
    }
}
