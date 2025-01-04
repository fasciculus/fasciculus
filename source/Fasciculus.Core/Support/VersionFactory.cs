namespace System
{
    /// <summary>
    /// Utility to create valid versions.
    /// </summary>
    public static class VersionFactory
    {
        /// <summary>
        /// Creates a valid version from the given string.
        /// </summary>
        public static Version Create(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                return new();
            }

            int dashIndex = version.IndexOf('-');

            if (dashIndex >= 0)
            {
                version = version.Substring(0, dashIndex);
            }

            if (string.IsNullOrEmpty(version))
            {
                return new();
            }

            int dotIndex = version.IndexOf('.');

            if (dotIndex < 0)
            {
                version += ".0";
            }

            return new(version);
        }
    }
}
