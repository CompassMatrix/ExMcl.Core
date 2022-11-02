using Microsoft;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Windows.Forms;
using WPFLauncher.Common;
using WPFLauncher.Manager;

namespace Microsoft.Event
{
    internal class LocalGameNetWork : IMethodHook
    {

        [OriginalMethod]
        public static byte[] a_Original(params object[] kvh) => null;

        
        [OriginalMethod]
        public void b_Original(ushort fls, byte[] flt)
        { }

        public static string GetMiddleStr(string oldStr, string preStr, string nextStr)
        {
            string tempStr = oldStr.Substring(oldStr.IndexOf(preStr) + preStr.Length);
            tempStr = tempStr.Substring(0, tempStr.IndexOf(nextStr));
            return tempStr;
        }

        public static T GetPrivateField<T>(object instance, string fieldname)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            Type type = instance.GetType();
            FieldInfo field = type.GetField(fieldname, flag);
            return (T)field.GetValue(instance);
        }

        public static string GetCommandLineArgs(Process process)
        {
            if (process is null) throw new ArgumentNullException(nameof(process));

            try
            {
                return GetCommandLineArgsCore();
            }
            catch (Win32Exception ex) when ((uint)ex.ErrorCode == 0x80004005)
            {
                // 没有对该进程的安全访问权限。
                return string.Empty;
            }
            catch (InvalidOperationException)
            {
                // 进程已退出。
                return string.Empty;
            }

            string GetCommandLineArgsCore()
            {
                using (var searcher = new ManagementObjectSearcher(
                           "SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
                using (var objects = searcher.Get())
                {
                    var @object = objects.Cast<ManagementBaseObject>().SingleOrDefault();
                    return @object?["CommandLine"]?.ToString() ?? "";
                }
            }
        }
    }
}