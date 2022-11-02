using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WPFLauncher.Manager;

namespace DotNetDetour.Extensions
{
    public static class ProcessExtensions
    {
        [DllImport("kernel32.dll")]
        static extern int OpenProcess(int a_, int Handle, int dwProcessId);

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(int hObject);

        [DllImport("ntdll.dll")]
        static extern int ZwWow64WriteVirtualMemory64(int hProcess, ulong pMemAddress, byte[] Buffer, ulong nSize, out ulong nReturnSize);

        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
        public static extern int WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int nSize, int lpNumberOfBytesWritten);

        public static int writeBytes(this akh process, ulong address, byte[] buffer, ulong size)
        {
            int num = OpenProcess(2035711, 0, process.ProcessId);
            int retn = ZwWow64WriteVirtualMemory64
                (num, address, buffer, size, out ulong ret);
            CloseHandle(num);
            return retn;
        }

        public static int writeBytes(this akh process, int address, byte[] buffer, int size)
        {
            int num = OpenProcess(2035711, 0, process.ProcessId);
            int retn = WriteProcessMemory(num, address, buffer, size, 0);
            CloseHandle(num);
            return retn;
        }
    }
}
