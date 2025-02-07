using Fasciculus.Blog.Preview.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Blog.Preview.Controllers
{
    public class BlogController : Controller
    {
        public BlogController()
        {
        }

        [Route("/")]
        public IActionResult Index()
        {
            ViewModel model = new()
            {
                Title = "Index"
            };

            return View("Index", model);
        }
    }
}
