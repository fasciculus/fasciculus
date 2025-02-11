using Fasciculus.Docs.Preview.Models;
using Fasciculus.Docs.Preview.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Fasciculus.Docs.Preview.Controllers
{
    public class DebugController : Controller
    {
        private readonly ContentClient client;
        private readonly IServiceCollection services;

        public DebugController(ContentClient client, IServiceCollection services)
        {
            this.client = client;
            this.services = services;
        }

        [Route("/Debug/")]
        public IActionResult Index()
        {
            ViewModel model = new()
            {
                Title = "Debug",
                Version = client.GetVersion(),
            };

            return View("Index", model);
        }

        [Route("/Debug/Services")]
        public IActionResult Services()
        {
            DebugServicesViewModel model = new()
            {
                Title = "Services",
                Version = client.GetVersion(),
                Descriptors = [.. services.OrderBy(s => s.ServiceType.FullName)],
            };

            return View("Services", model);
        }
    }
}
