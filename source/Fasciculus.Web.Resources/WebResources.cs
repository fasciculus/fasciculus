using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace Fasciculus.Web.Resources
{
    public static class WebResources
    {
        private static readonly Assembly assembly = typeof(WebResources).Assembly;

        public static EmbeddedFileProvider BootstrapProvider
            => new(assembly, "Fasciculus.Web.Resources.Bootstrap");

        public static EmbeddedFileProvider KatexProvider
            => new(assembly, "Fasciculus.Web.Resources.Katex");

        public static StaticFileOptions BootstrapOptions
            => new() { FileProvider = BootstrapProvider };

        public static StaticFileOptions KatexOptions
            => new() { FileProvider = KatexProvider };

        public static IApplicationBuilder UseBootstrapResources(this IApplicationBuilder app)
            => app.UseStaticFiles(BootstrapOptions);

        public static IApplicationBuilder UseKatexResources(this IApplicationBuilder app)
            => app.UseStaticFiles(KatexOptions);
    }
}
