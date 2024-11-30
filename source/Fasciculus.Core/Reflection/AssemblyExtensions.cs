namespace System.Reflection
{
    public static class AssemblyExtensions
    {
        public static string GetVersion(this Assembly? assembly)
        {
            AssemblyName? name = assembly?.GetName();
            Version? version = name?.Version;

            return version?.ToString() ?? string.Empty;
        }
    }
}
