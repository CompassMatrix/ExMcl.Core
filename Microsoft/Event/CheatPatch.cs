using Microsoft;
using WPFLauncher.Model;

namespace Microsoft.Event
{
    internal class CheatPatch : IMethodHook
    {
        [HookMethod("WPFLauncher.Network.Protocol.zs")]
        public static void d(string gfr, object gfs, uint gft = 0U)
        { }

        [HookMethod("WPFLauncher.Model.CompGameM")]
        public void f(byte[] hmr)
        { }

        [HookMethod("WPFLauncher.Model.CompGameM")]
        public void i(byte[] hmu)
        { }

        [HookMethod("WPFLauncher.Manager.aix")]
        public void l(string jhe = "No Exception Description\r\n", int jhf = 0)
        { }

        [HookMethod("WPFLauncher.Manager.aku")]
        public void e(GameM kbs)
        {
            if (kbs != null)
            {
                if (kbs.Type == GType.NET_GAME || kbs.Type == GType.SERVER_GAME || kbs.Type == GType.ONLINE_LOBBY_GAME)
                {
                    kbs.Status = GStatus.PLAYING;
                }
                else
                {
                    e_Original(kbs);
                }
            }
        }
        [OriginalMethod]
        public void e_Original(GameM kbs)
        { }
    }
}