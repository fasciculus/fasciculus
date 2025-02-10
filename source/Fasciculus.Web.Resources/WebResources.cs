using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace Fasciculus.Web.Resources
{
    public static class WebResources
    {
        private static readonly Assembly assembly = typeof(WebResources).Assembly;

        public static EmbeddedFileProvider Test()
        {
            return new EmbeddedFileProvider(assembly, "Test");
        }

        public static EmbeddedFileProvider Bootstrap()
        {
            return new EmbeddedFileProvider(assembly, "Bootstrap");
        }
    }
}
