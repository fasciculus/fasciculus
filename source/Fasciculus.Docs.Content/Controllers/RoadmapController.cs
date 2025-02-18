using Fasciculus.Docs.Content.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Docs.Content.Controllers
{
    public class RoadmapController : Controller
    {
        private readonly RoadmapFiles files;

        public RoadmapController(RoadmapFiles files)
        {
            this.files = files;
        }

        [Route("/Roadmap/Keys")]
        public string Keys()
        {
            return string.Join(",", files.GetKeys());
        }

        [Route("/Roadmap/File/{key}")]
        public string Entry(string key)
        {
            return files.GetFile(key).FullName;
        }
    }
}
