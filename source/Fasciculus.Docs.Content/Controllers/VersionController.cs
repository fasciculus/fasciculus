using Fasciculus.Docs.Content.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Docs.Content.Controllers
{
    public class VersionController : Controller
    {
        private readonly VersionProvider version;

        public VersionController(VersionProvider version)
        {
            this.version = version;
        }

        [Route("/Version")]
        public string Version()
        {
            return version.GetVersion().ToBinary().ToString();
        }
    }
}
