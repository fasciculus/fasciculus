using Fasciculus.Docs.Preview.Models;
using Fasciculus.Docs.Preview.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Docs.Preview.Controllers
{
    public class MainController : Controller
    {
        private readonly ContentClient client;

        public MainController(ContentClient client)
        {
            this.client = client;
        }

        [Route("/")]
        public IActionResult Index()
        {
            ViewModel model = new()
            {
                Title = "Preview",
                Version = client.GetVersion(),
            };

            return View("Index", model);
        }
    }
}
