using Fasciculus.Docs.Content.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Docs.Content.Controllers
{
    public class SpecificationController : Controller
    {
        private readonly SpecificationFiles specificationFiles;

        public SpecificationController(SpecificationFiles specificationFiles)
        {
            this.specificationFiles = specificationFiles;
        }

        [Route("/Specifications/Keys")]
        public string Keys()
        {
            return string.Join(",", specificationFiles.GetKeys());
        }

        [Route("/Specifications/File/{key}")]
        public string Entry(string key)
        {
            return specificationFiles.GetFile(key).FullName;
        }
    }
}
