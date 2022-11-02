using Microsoft;
using System;
using System.Linq;
using System.Net;

namespace Microsoft.Event
{
    internal class BaseInformation : IMethodHook
    {
        public static string rand(int len)
        {
            Random random = new Random();
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRST123456789", len)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HookMethod("WPFLauncher.Manager.Log.Util.amw")]
        public static string b()
        {
            return rand(12);
        }

        [HookMethod("WPFLauncher.Manager.akd")]
        private string d(string jou)
        {
            var result = rand(16);
            result += jou;
            if (jou.Length > 24)
            {
                result = result.Substring(0, 24);
            }
            return result;
        }

        [HookMethod("WPFLauncher.Manager.akd")]
        public string f()
        {
            return rand(8);
        }

        [HookMethod("WPFLauncher.Manager.akd")]
        public string g()
        {
            var data = new byte[4];
            new Random().NextBytes(data);
            return new IPAddress(data).ToString();
        }

        [HookMethod("WPFLauncher.ba")]
        public static string an()
        {
            var data = new byte[4];
            new Random().NextBytes(data);
            return $"netdns=127.0.0.1 gw={new IPAddress(data).ToString()} gwdns=correct";
        }
    }
}