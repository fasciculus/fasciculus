using Fasciculus.Docs.Content.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Docs.Content.Controllers
{
    public class PackageController : Controller
    {
        private readonly PackageFiles packageFiles;

        public PackageController(PackageFiles packageFiles)
        {
            this.packageFiles = packageFiles;
        }

        [Route("/Packages/Keys")]
        public string Keys()
        {
            return string.Join(",", packageFiles.GetKeys());
        }

        [Route("/Packages/File/{key}")]
        public string Entry(string key)
        {
            return packageFiles.GetFile(key).FullName;
        }

    }
}
