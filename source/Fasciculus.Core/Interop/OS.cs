using System.Runtime.InteropServices;

namespace Fasciculus.Interop
{
    public static class OS
    {
        public static bool IsWindows
        {
            get => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }
    }
}
