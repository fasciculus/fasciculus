using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace Fasciculus.Site
{
    public static class SiteResources
    {
        private static readonly Assembly assembly = typeof(SiteResources).Assembly;

        public static EmbeddedFileProvider Provider
            => new(assembly, "Fasciculus.Site.Resources");

        public static StaticFileOptions Options
            => new() { FileProvider = Provider };

        public static IApplicationBuilder UseSiteResources(this IApplicationBuilder app)
            => app.UseStaticFiles(Options);
    }
}
