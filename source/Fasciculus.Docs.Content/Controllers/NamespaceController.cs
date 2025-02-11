using Fasciculus.Docs.Content.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fasciculus.Docs.Content.Controllers
{
    public class NamespaceController : Controller
    {
        private readonly NamespaceFiles namespaceFiles;

        public NamespaceController(NamespaceFiles namespaceFiles)
        {
            this.namespaceFiles = namespaceFiles;
        }

        [Route("/Namespaces/Keys")]
        public string Keys()
        {
            return string.Join(",", namespaceFiles.GetKeys());
        }

        [Route("/Namespaces/File/{key}")]
        public string Entry(string key)
        {
            return namespaceFiles.GetFile(key).FullName;
        }
    }
}
