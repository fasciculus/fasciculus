namespace Microsoft.Extensions.Hosting
{
    public static class DI
    {
        public static HostApplicationBuilder CreateEmptyBuilder()
        {
            HostApplicationBuilderSettings settings = new() { DisableDefaults = true };

            return Host.CreateEmptyApplicationBuilder(settings);
        }
    }
}
