namespace Microsoft.Extensions.Hosting
{
    public static class ApplicationHost
    {
        public static HostApplicationBuilder CreateEmptyBuilder()
        {
            HostApplicationBuilderSettings settings = new() { DisableDefaults = true };

            return new HostApplicationBuilder(settings);
        }
    }
}
