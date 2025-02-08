using Fasciculus.Blog.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Xml.Linq;

namespace Fasciculus.Blog.Controllers
{
    public class BlogController : Controller
    {
        private readonly Entries entries;
        private readonly Graphics graphics;

        public BlogController(Entries entries, Graphics graphics)
        {
            this.entries = entries;
            this.graphics = graphics;
        }

        [Route("/Version")]
        public string Version()
        {
            FileInfo file = new(GetType().Assembly.Location);

            return file.LastWriteTime.ToBinary().ToString();
        }

        [Route("/Keys")]
        public string Keys()
        {
            return string.Join(",", entries.GetKeys());
        }

        [Route("/Entry/{key}")]
        public string Entry(string key)
        {
            return entries.GetEntry(key);
        }

        [Route("/Graphic/{key}")]
        public string Graphic(string key)
        {
            XElement? graphic = graphics.GetSvg(key);

            return graphic is null ? string.Empty : graphic.ToString();
        }
    }
}
