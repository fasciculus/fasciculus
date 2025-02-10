using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace Fasciculus.Web.Resources
{
    public static class WebResources
    {
        private static readonly Assembly assembly = typeof(WebResources).Assembly;

        public static EmbeddedFileProvider TestProvider
            => new EmbeddedFileProvider(assembly, "Fasciculus.Web.Resources.Test");

        public static EmbeddedFileProvider BootstrapProvider
            => new EmbeddedFileProvider(assembly, "Fasciculus.Web.Resources.Bootstrap");

        public static StaticFileOptions TestOptions
            => new() { FileProvider = TestProvider };

        public static StaticFileOptions BootstrapOptions
            => new() { FileProvider = BootstrapProvider };

        public static IApplicationBuilder UseTestResources(this IApplicationBuilder app)
            => app.UseStaticFiles(TestOptions);

        public static IApplicationBuilder UseBootstrapResources(this IApplicationBuilder app)
            => app.UseStaticFiles(BootstrapOptions);
    }
}
