using Microsoft;
using WPFLauncher.Model;

namespace Microsoft.Event
{
    internal class AcSdkWrapper : IMethodHook
    {
        [HookMethod("WPFLauncher.Manager.aku")]
        private void o()
        { }

        [HookMethod("WPFLauncher.Manager.Configuration.apu")]
        public bool get_PlayCG() => false;

        [HookMethod("WPFLauncher.Manager.akm")]
        private void d(acv jxc)
        { }
        [OriginalMethod]
        private void d_Original(acv jxc)
        { }

        [HookMethod("WPFLauncher.Manager.akm")]
        private void e(acv jxd)
        {
            d_Original(jxd);
        }

        [HookMethod("WPFLauncher.Util.rz")]
        public static bool r(string eub)
        {
            return eub.Contains("\\Game\\.minecraft\\shaderpacks") || eub.Contains("\\Game\\.minecraft\\resourcepacks") ? true : r_Original(eub);
        }
        [OriginalMethod]
        public static bool r_Original(string eub) => false;
    }
}