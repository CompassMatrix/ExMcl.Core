using DotNetDetour.Extensions;
using Mcl.Core.Network;
using Mcl.Core.Network.Interface;
using Mcl.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WPFLauncher.Common;
using WPFLauncher.Manager;
using WPFLauncher.Model;
using WPFLauncher.Network.Protocol;
using WPFLauncher.ViewModel.Launcher;

namespace Microsoft.Event
{
    internal class GameProcessces : IMethodHook
    {
        public static akh GameProcess;

        public static string GetMiddleStr(string oldStr, string preStr, string nextStr)
        {
            string tempStr = oldStr.Substring(oldStr.IndexOf(preStr) + preStr.Length);
            tempStr = tempStr.Substring(0, tempStr.IndexOf(nextStr));
            return tempStr;
        }

        [HookMethod("System.Diagnostics.Process")]
        public static Process[] GetProcesses()
        {
            Process[] processes_Original = GetProcesses_Original();
            List<Process> array = new List<Process>();
            string[] arraySystemProcess = new string[11]
            {
                "dllhost", "IAStorDataMgrSvc", "fontdrvhost", "WmiPrvSE", "svchost", "crss", "SecurityHealthService", "spoolsv", "dwm", "ctfmon",
                "conhost"
            };
            for (int num = 0; num <= processes_Original.Length; num++)
            {
                foreach (string value in arraySystemProcess)
                {
                    if (processes_Original[num].ProcessName.Equals(value))
                    {
                        array.Add(processes_Original[num]);
                    }
                }
            }
            return array.ToArray();
        }
        [OriginalMethod]
        public static Process[] GetProcesses_Original() => null;

        [HookMethod("WPFLauncher.Manager.aki")]
        public static List<string> n() => new List<string> { "Minecraft" };

        [HookMethod("WPFLauncher.Network.Protocol.zw")]
        public static NetRequestAsyncHandle f(string gqx, string gqy, Action<INetResponse> gqz = null, zt gra = zt.a, string grb = null)
        {
            return null;
        }
        
        [HookMethod("WPFLauncher.Util.rw")]
        public static akh a(string processPath, string processArgs, EventHandler onGameExit, akf gameType, string workingDirectory = null, bool isListenLogs = false, Action<string> onLogsOutPut = null)
        {

            GameProcessces.GameProcess = a_Original(processPath, processArgs,
            new EventHandler(delegate (object A_0, EventArgs A_1)
            {
                onGameExit(A_0, A_1);
            }), gameType, workingDirectory, true,
            delegate (string LogsStr)
            {
                if (string.IsNullOrEmpty(LogsStr)) { return; }
            });

            File.WriteAllBytes(Tool.getGamePath() + "\\ZeroDay_Locked.cach", Encoding.UTF8.GetBytes(processArgs));
            while (!File.Exists(Tool.getGamePath() + "\\ZeroDay_Start.cach"))
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }
            processArgs = Encoding.UTF8.GetString(File.ReadAllBytes(Tool.getGamePath() + "\\ZeroDay_Start.cach"));
            File.Delete(Tool.getGamePath() + "\\ZeroDay_Start.cach");

            new Thread(new ThreadStart(delegate
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    while (!GameProcessces.GameProcess.HasExited)
                    {
                        foreach (ProcessModule64 tagModule in ProcessModules.getWOW64Modules(GameProcess.ProcessId))
                        {
                            if (tagModule.ModuleName == "api-ms-win-crt-utility-l1-1-1.dll")
                            {
                                var lpWriteResult = GameProcessces.GameProcess.writeBytes((ulong)tagModule.BaseAddress + 200032, new byte[] { 176, 0, 195, 36, 16 }, 5); //registerTransformers
                                lpWriteResult += GameProcessces.GameProcess.writeBytes((ulong)tagModule.BaseAddress + 193200, new byte[] { 176, 1, 195, 36, 16 }, 5); //launchAfterGame
                                lpWriteResult += GameProcessces.GameProcess.writeBytes((ulong)tagModule.BaseAddress + 196464, new byte[] { 176, 0x10, 195 }, 3); //client_pcyc_check
                                lpWriteResult += GameProcessces.GameProcess.writeBytes((ulong)tagModule.BaseAddress + 315088, new byte[] { 176, 0x10, 195, 36, 16 }, 5); //SELECT ProcessId,Name,ExecutablePath,CommandLine FROM Win32_Process
                                lpWriteResult += GameProcessces.GameProcess.writeBytes((ulong)tagModule.BaseAddress + 214592, new byte[] { 176, 0x10, 195, 36, 16 }, 5); //wmic
                            }
                        }
                    }
                }
                else
                {
                    while (!GameProcessces.GameProcess.HasExited)
                    {
                        GameProcessces.GameProcess.Refresh();
                        foreach (ProcessModule tagModule in GameProcessces.GameProcess.Modules)
                        {
                            if (tagModule.ModuleName == "api-ms-win-crt-utility-l1-1-1.dll")
                            {
                                var lpWriteResult = GameProcessces.GameProcess.writeBytes((int)tagModule.BaseAddress + 200032, new byte[] { 176, 0, 195, 36, 16 }, 5); //registerTransformers
                                lpWriteResult += GameProcessces.GameProcess.writeBytes((int)tagModule.BaseAddress + 193200, new byte[] { 176, 1, 195, 36, 16 }, 5); //launchAfterGame
                                lpWriteResult += GameProcessces.GameProcess.writeBytes((int)tagModule.BaseAddress + 196464, new byte[] { 176, 0x10, 195 }, 3); //client_pcyc_check
                                lpWriteResult += GameProcessces.GameProcess.writeBytes((int)tagModule.BaseAddress + 315088, new byte[] { 176, 0x10, 195, 36, 16 }, 5); //SELECT ProcessId,Name,ExecutablePath,CommandLine FROM Win32_Process
                                lpWriteResult += GameProcessces.GameProcess.writeBytes((int)tagModule.BaseAddress + 214592, new byte[] { 176, 0x10, 195, 36, 16 }, 5); //wmic
                            }
                        }
                    }
                }
            }))
            { IsBackground = true, }.Start();

            return GameProcessces.GameProcess;
        }

        [OriginalMethod]
        public static akh a_Original(string esk, string esl, EventHandler esm, akf esn, string eso = null, bool esp = false, Action<string> esq = null) => null;

    }
}