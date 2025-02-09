using Fasciculus.Docs.Content.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Docs.Content.Controllers
{
    public class VersionController : Controller
    {
        private readonly ContentVersion version;

        public VersionController(ContentVersion version)
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
