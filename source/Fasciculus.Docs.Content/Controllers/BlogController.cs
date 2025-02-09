using Fasciculus.Docs.Content.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Docs.Content.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogFiles blogFiles;

        public BlogController(BlogFiles blogFiles)
        {
            this.blogFiles = blogFiles;
        }

        [Route("/Blog/Keys")]
        public string Keys()
        {
            return string.Join(",", blogFiles.GetKeys());
        }

        [Route("/Blog/File/{key}")]
        public string Entry(string key)
        {
            return blogFiles.GetFile(key).FullName;
        }
    }
}
