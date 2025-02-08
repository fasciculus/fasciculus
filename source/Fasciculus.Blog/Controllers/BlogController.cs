using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Fasciculus.Blog.Controllers
{
    public class BlogController : Controller
    {
        [Route("/Version")]
        public string Version()
        {
            FileInfo file = new(GetType().Assembly.Location);

            return file.LastWriteTime.ToBinary().ToString();
        }

        public string Files()
        {
            return string.Empty;
        }
    }
}
