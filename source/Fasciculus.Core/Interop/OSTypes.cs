using System.Runtime.InteropServices;

namespace Fasciculus.Interop
{
    public static class OSTypes
    {
        public static readonly OSType Current = GetCurrent();

        public static readonly bool IsLinux = Current == OSType.Linux;
        public static readonly bool IsOSX = Current == OSType.OSX;
        public static readonly bool IsWindows = Current == OSType.Windows;

        private static OSType GetCurrent()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSType.Linux;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSType.OSX;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSType.Windows;
            }

            return OSType.Unknown;
        }
    }
}
