using System;
using System.Runtime.InteropServices;

namespace Microsoft
{
    public class NativeAPI
    {
        [DllImport("kernel32")]
        public static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, Protection flNewProtect, out uint lpflOldProtect);
    }
}
