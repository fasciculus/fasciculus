using System.Runtime.InteropServices;

namespace Fasciculus.Interop
{
    /// <summary>
    /// OS information.
    /// </summary>
    public static class OS
    {
        /// <summary>
        /// Whether the corrent OS is windows.
        /// <para>
        /// Shorthand for <c>RuntimeInformation.IsOSPlatform(OSPlatform.Windows)</c>.
        /// </para>
        /// </summary>
        public static bool IsWindows
        {
            get => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }
    }
}
