using Fasciculus.Docs.Content.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Docs.Content.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogContent blog;

        public BlogController(BlogContent blog)
        {
            this.blog = blog;
        }

        [Route("/Blog/Keys")]
        public string Keys()
        {
            return string.Join(",", blog.GetKeys());
        }

        [Route("/Blog/Entry/{key}")]
        public string Entry(string key)
        {
            return blog.GetEntry(key);
        }
    }
}
